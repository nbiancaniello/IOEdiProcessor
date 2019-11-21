using IOEdiProcessor.Data;
using IOEdiProcessor.Data.Context;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Text;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace IOEdiProcessor.Logic
{
    public class EdifactLogic
    {
        private readonly IOEdiProcessorContext _context;
        private readonly static Dictionary<string, string> _parse = new Dictionary<string, string>
        {
            { "á","a" },
            { "é","e" },
            { "í","i" },
            { "ó","o" },
            { "ú","u" },
            { "ñ","n" },
            { "¼", " 1/4 "},
            { "½", " 2/4 "},
            { "¾", " 3/4 "}
        };

        public EdifactLogic(IOEdiProcessorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the EDI file
        /// </summary>
        /// <param name="shipPlanId"></param>
        /// <param name="custID"></param>
        /// <param name="docType"></param>
        /// <param name="PurposeCode"></param>
        /// <returns></returns>
        public async Task<string> GenerateEDIFile(string shipPlanId, string custID, string docType, string PurposeCode)
        {
            // Mapping information
            string fileContent = null;
            MapInfo map = await getMapInfo(custID, docType);
            if (map != null)
            {
                if (map.Position.Count > 0)
                {
                    // Flat File Generation
                    List<Edifact> EdifactList = await GetEdifact(shipPlanId, custID, docType, PurposeCode, map.Header.LeadingZero);
                    fileContent = CreateLines(EdifactList, map);
                }
                else
                {
                    fileContent = "NO MAPPING POSITIONS FOR CUSTOMER: " + custID + ", DOCTYPE: " + docType;
                }                
            } else
            {
                fileContent = "NO MAPPING HEADER FOR CUSTOMER: " + custID + ", DOCTYPE: " + docType;
            }
            return fileContent;
        }

        protected async Task<MapInfo> getMapInfo(string custID, string docType)
        {
            MapInfo map = new MapInfo();
            try
            {
                //using (DbConnection con = _context.Database.GetDbConnection())
                var con = (SqlConnection)_context.Database.GetDbConnection();
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT HEADER_ID, CUSTOMER_ID, PARTNER_ID, STANDARD, VERSION, TRANSACTION_SET, TEST_PROD, LEADING_ZERO FROM EDI_HEADERS WHERE CUSTOMER_ID = '{custID}' AND TRANSACTION_SET = '{docType}'";
                    await con.OpenAsync();
                    using (DbDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            map.Header = new Header
                            {
                                ID = int.Parse(dr["HEADER_ID"].ToString()),
                                CustomerID = dr["CUSTOMER_ID"].ToString(),
                                PartnerID = dr["PARTNER_ID"].ToString(),
                                Standard = dr["STANDARD"].ToString(),
                                Version = dr["VERSION"].ToString(),
                                TransactionSet = dr["TRANSACTION_SET"].ToString(),
                                TestProd = dr["TEST_PROD"].ToString(),
                                LeadingZero = dr["LEADING_ZERO"].ToString()
                            };
                        }
                    }

                    if (map != null)
                    {
                        cmd.CommandText = $@"SELECT TAG_NAME, LENGTH FROM EDI_POSITIONS WHERE FK_HEADER_ID = '{map.Header.ID}'";
                        using (DbDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            map.Position = new Dictionary<string, int>();
                            while (await dr.ReadAsync())
                            {
                                map.Position.Add(dr["TAG_NAME"].ToString(), Int16.Parse(dr["LENGTH"].ToString()));
                            }
                        }
                    }

                    con.Close();
                    //con.Dispose();
                }
            }
            catch (DbException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return map;
        }

        protected async Task<List<Edifact>> GetEdifact(string shipPlanId, string custID, string docType, string PurposeCode, string LeadingZero)
        {
            List<Edifact> list = new List<Edifact>();
            string mbolID = null;

            string queryStr = $@"SELECT DISTINCT M.MBOL_ID,
		                            M.MBOL_NUMBER,
                                    P.PACK_LIST_ID,
		                            P.PACK_LIST_NUMBER,
		                            DSP.PLN_SHIP_DATE,
                                    M.SHIP_TIME,
		                            C.CUST_ID [BUYER_ID], 
		                            C.NAME [BUYER_NAME],
		                            C.ADDR_1 [BUYER_ADDRESS1],
                                    C.ADDR_2 [BUYER_ADDRESS2],
		                            C.CITY [BUYER_CITY],
		                            C.POSTAL_CODE [BUYER_PCODE],
		                            C.COUNTRY [BUYER_COUNTRY],
		                            C.PHONE [BUYER_PHONE],
		                            CST.PLANT_ID [SHIP_TO_ID],
                                    CST.PLANT_NAME [SHIP_TO_NAME],
		                            CST.PLANT_ADDR_1 [SHIP_TO_ADDRESS1],
                                    CST.PLANT_ADDR_2 [SHIP_TO_ADDRESS2],
		                            CST.PLANT_ADDR_3 [SHIP_TO_ADDRESS3],
		                            CST.PLANT_CITY [SHIP_TO_CITY],
                                    CST.PLANT_PROV_STATE [SHIP_TO_PROV],
		                            CST.PLANT_POSTALCODE [SHIP_TO_PCODE],
		                            CST.PLANT_COUNTRY [SHIP_TO_COUNTRY],";

            if (custID == "VWT" | custID == "DELP" | custID == "DELP_CN")
            {
                queryStr = queryStr + @"PP.DOCK_CODE[SHIP_TO_DOCK_CODE], PP.DROP_ZONE[SHIP_TO_DROP_ZONE],";
            }

            queryStr = queryStr + $@"CST.ULTIMATE_DESTINATION [SHIP_TO_UDESTINATION],
                                CST.INVOICE_TO_CODE [CUST_SHIP_TO_ID],
		                        CC.FIRST_NAME + ' ' + CC.LAST_NAME [CONTACT_NAME],
		                        CBT.NAME [BILL_TO_NAME],
		                        CBT.ADDR_1 [BILL_TO_ADDRESS1],
                                CBT.ADDR_2 [BILL_TO_ADDRESS2],
		                        CBT.ADDR_3 [BILL_TO_ADDRESS3],
		                        CBT.POSTALCODE [BILL_TO_PCODE],
		                        CBT.COUNTRY [BILL_TO_COUNTRY],
                                CI.COMPANY_ID [COMPANY_ID],
                                CI.COMPANY_NAME [COMPANY_NAME],
		                        CI.COMPANY_ADDRESS1 [SELLER_ADDRESS1],
                                CI.COMPANY_ADDRESS2 [SELLER_ADDRESS2],
		                        CI.COMPANY_CITY [SELLER_CITY],
                                CI.COMPANY_PROV [SELLER_PROV],
		                        CI.COMPANY_PCODE [SELLER_PCODE],
		                        M.TRANSPORT_COMPANY_SCAC [SCAC],
                                TC.TRANSPORT_COMPANY_NAME [SCAC_NAME],
                                M.FREIGHT_BILL_TO_TERMS [TERMS_OF_DELIVERY],
		                        M.TRANSPORT_METHOD,
                                M.EQUIPMENT_NUMBER [TRACKING_NUMBER],
		                        M.PRO_NUMBER [TRAILER_ID],
		                        M.EQUIPMENT_DESCRIPTION,
		                        E.ENTITY_ID [SHIP_FROM_ID],
		                        E.NAME [SHIP_FROM_NAME],
		                        E.ADDR_1 [SHIP_FROM_ADDRESS1],
		                        E.ADDR_2 [SHIP_FROM_ADDRESS2],
		                        E.ADDR_3 [SHIP_FROM_ADDRESS3],
		                        E.POSTAL_CODE [SHIP_FROM_PCODE],
		                        E.COUNTRY [SHIP_FROM_COUNTRY],
		                        E.CITY [SHIP_FROM_CITY],
                                E.PROV_STATE [SHIP_FROM_PROV],
		                        E.PHONE [SHIP_FROM_PHONE],
                                SI.SUPPLIER_ID [SHIP_FROM_SUPPLIER_ID],
                                CASE WHEN C.CUST_ID = 'VWT' THEN DSP.REQ_DEL_DATE ELSE null END AS REQ_DEL_DATE,
				                DSP.CALL_OFF_NUM,
				                DSP.MAT_ISSUER
                            FROM MBOL M
	                        LEFT JOIN PACK_LIST P ON M.MBOL_ID = P.MBOL_ID
	                        LEFT JOIN CUSTOMER_SHIP_TO CST ON M.SHIP_TO_ID = CST.PLANT_ID";

            if (custID == "VWT" | custID == "DELP" | custID == "DELP_CN")
            {
                queryStr = queryStr + $@" LEFT JOIN PART_PLANT PP ON CST.PLANT_ID = PP.fk_SHIP_TO_ID";
            }

            queryStr = queryStr + $@" LEFT JOIN CUSTOMER C ON C.CUST_ID = CST.FK_CUST_ID
	                            LEFT JOIN CUSTOMER_BILL_TO CBT ON M.FREIGHT_BILL_TO_CODE = CBT.BILL_TO
	                            LEFT JOIN CUSTOMER_CONTACTS CC ON P.CUST_CONTACT = CC.CONTACT_ID
	                            LEFT JOIN TRANSPORT_COMPANY TC ON M.FK_TRANS_COMPANY_ID = TC.TRANSPORT_COMPANY_ID
	                            LEFT JOIN ENTITY E ON M.SHIP_FROM = E.ENTITY_ROW_ID
                                LEFT JOIN SUPPLIER_INFO SI ON SI.fk_ENTITY_ROW_ID = E.ENTITY_ROW_ID
                                LEFT JOIN DAILY_SHIP_PLAN DSP ON DSP.SHIP_PLANID = M.FK_SHIP_PLANID
	                            , ACC_COMP_INFO CI
                                WHERE M.FK_SHIP_PLANID = '{shipPlanId}' AND C.CUST_ID = '{custID}' AND SI.CUST_ID = '{custID}';";

            try
            {
                //using (DbConnection con = _context.Database.GetDbConnection())
                var con = (SqlConnection)_context.Database.GetDbConnection();
                using (DbCommand cmd = con.CreateCommand())
                {
                    await con.OpenAsync();
                    cmd.CommandText = queryStr;
                    using (DbDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            mbolID = dr["MBOL_ID"].ToString();
                            string packListID = dr["PACK_LIST_ID"].ToString();
                            DateTime reqDelDate = new DateTime();
                            Edifact Edi = new Edifact
                            {
                                CustomerId = custID,
                                CustomerName = dr["BUYER_NAME"].ToString(),
                                DocHeader = new DocHeader
                                {
                                    DocumentType = docType,
                                    ASNType = (PurposeCode == "00") ? "ORIGINAL" : "CANCELLATION",
                                    ShipmentNumber = dr["MBOL_NUMBER"].ToString(),
                                    PackListNumber = (dr["PACK_LIST_NUMBER"].ToString() != null) ? dr["PACK_LIST_NUMBER"].ToString() : "000000000000000",
                                    Date = DateTime.Now.ToString("yyyyMMdd"),
                                    Time = DateTime.Now.ToString("HHmm"),
                                    CallOffNumber = dr["CALL_OFF_NUM"].ToString(),
                                    MaterialIssuer = dr["MAT_ISSUER"].ToString()
                                },
                                ShipTo = new ShipTo
                                {
                                    ShipToID = dr["SHIP_TO_ID"].ToString(),
                                    ShipToName = dr["SHIP_TO_NAME"].ToString(),
                                    Address1 = ParseText(dr["SHIP_TO_ADDRESS1"].ToString()),
                                    Address2 = ParseText(dr["SHIP_TO_ADDRESS2"].ToString()),
                                    Address3 = ParseText(dr["SHIP_TO_ADDRESS3"].ToString()),
                                    City = ParseText(dr["SHIP_TO_CITY"].ToString()),
                                    Province = ParseText(dr["SHIP_TO_PROV"].ToString()),
                                    PostalCode = dr["SHIP_TO_PCODE"].ToString(),
                                    Country = ParseText(dr["SHIP_TO_COUNTRY"].ToString()),
                                    ShipToDockCode = (custID == "VWT" | custID == "DELP" | custID == "DELP_CN") ? dr["SHIP_TO_DOCK_CODE"].ToString() : "",
                                    ShipToDropZone = (custID == "VWT" | custID == "DELP" | custID == "DELP_CN") ? dr["SHIP_TO_DROP_ZONE"].ToString() : "",
                                    UltimateDestination = dr["SHIP_TO_UDESTINATION"].ToString(),
                                    CustomerShiptToID = dr["CUST_SHIP_TO_ID"].ToString()
                                },
                                ShipFrom = new ShipFrom
                                {
                                    ShipFromID = dr["SHIP_FROM_ID"].ToString(),
                                    ShipFromName = dr["SHIP_FROM_NAME"].ToString(),
                                    Address1 = ParseText(dr["SHIP_FROM_ADDRESS1"].ToString()),
                                    Address2 = ParseText(dr["SHIP_FROM_ADDRESS2"].ToString()),
                                    Address3 = ParseText(dr["SHIP_FROM_ADDRESS3"].ToString()),
                                    PostalCode = dr["SHIP_FROM_PCODE"].ToString(),
                                    City = ParseText(dr["SHIP_FROM_CITY"].ToString()),
                                    Province = ParseText(dr["SHIP_FROM_PROV"].ToString()),
                                    Country = ParseText(dr["SHIP_FROM_COUNTRY"].ToString()),
                                    DunsNumber = "259898559", //Hardcoded, ToDo: create a table and save this data.
                                    ShipFromSupplierID = dr["SHIP_FROM_SUPPLIER_ID"].ToString()
                                },
                                SoldTo = new SoldTo
                                {
                                    SoldToID = dr["BUYER_ID"].ToString(),
                                    SoldToName = ParseText(dr["BUYER_NAME"].ToString()),
                                    Address1 = ParseText(dr["BUYER_ADDRESS1"].ToString()),
                                    Address2 = ParseText(dr["BUYER_ADDRESS2"].ToString()),
                                    City = ParseText(dr["BUYER_CITY"].ToString()),
                                    PostalCode = dr["BUYER_PCODE"].ToString(),
                                    Country = ParseText(dr["BUYER_COUNTRY"].ToString())
                                },
                                Seller = new Seller
                                {
                                    CompanyID = dr["COMPANY_ID"].ToString(),
                                    CompanyName = dr["COMPANY_NAME"].ToString(),
                                    Address1 = ParseText(dr["SELLER_ADDRESS1"].ToString()),
                                    Address2 = ParseText(dr["SELLER_ADDRESS2"].ToString()),
                                    City = ParseText(dr["SELLER_CITY"].ToString()),
                                    Province = ParseText(dr["SELLER_CITY"].ToString()),
                                    PostalCode = dr["SELLER_PCODE"].ToString()
                                },
                                Freight = new Freight
                                {
                                    SCACCode = dr["SCAC"].ToString(),
                                    CarrierName = dr["SCAC_NAME"].ToString(),
                                    ForwarderDunsNumber = "123456789", //Hardcoded, ToDo: create a table and save this data.
                                    TermsOfDelivery = dr["TERMS_OF_DELIVERY"].ToString(),
                                    TrackingNumber = dr["TRACKING_NUMBER"].ToString(),
                                    ProNumber = dr["TRAILER_ID"].ToString(),
                                    TransportationMethod = dr["TRANSPORT_METHOD"].ToString(),
                                    EquipmentDescription = dr["EQUIPMENT_DESCRIPTION"].ToString()
                                },
                                ShipDates = new ShipDates
                                {
                                    ShipDate = DateTime.Parse(dr["PLN_SHIP_DATE"].ToString()).ToString("yyyyMMdd"),
                                    ShipTime = DateTime.Parse(dr["SHIP_TIME"].ToString()).ToString("HHmm")
                                },
                                ReqDates = new ReqDates
                                {
                                    ReqDelDate = (DateTime.TryParse(dr["REQ_DEL_DATE"].ToString(), out reqDelDate)) ? reqDelDate.ToString("yyyyMMdd") : null
                                }

                            };

                            // VWT doesn't want leading zeroes...
                            if (LeadingZero == "0")
                            {
                                Edi.DocHeader.PackListNumber = Int32.Parse(Edi.DocHeader.PackListNumber).ToString("###############");
                            }

                            string pieces = null; // ToDo: It doesn't apply for Brose, change logic to bring qty of pieces by part

                            //using (DbConnection con1 = _context.Database.GetDbConnection())
                            var con1 = (SqlConnection)_context.Database.GetDbConnection();
                            using (DbCommand cmd1 = con1.CreateCommand())
                            {
                                cmd1.CommandText = @"MBOL_SELECT_ONE";
                                cmd1.CommandType = System.Data.CommandType.StoredProcedure;
                                var param = cmd1.CreateParameter();
                                param.ParameterName = "@fk_MBOL_ID";
                                param.Value = mbolID;
                                cmd1.Parameters.Add(param);
                                //con1.Open();
                                using (DbDataReader dr1 = await cmd1.ExecuteReaderAsync())
                                {
                                    while (await dr1.ReadAsync())
                                    {
                                        Measure measure = new Measure
                                        {
                                            GrossWeightKG = (double.Parse(dr1["GROSS_WEIGHT"].ToString()) * 0.45359).ToString("#####0"),
                                            GrossWeightLB = ((double)dr1["GROSS_WEIGHT"]).ToString("#####0"),
                                            NetWeightKG = (double.Parse(dr1["NET_WEIGHT"].ToString()) * 0.45359).ToString("#####0"),
                                            NetWeightLB = ((double)dr1["NET_WEIGHT"]).ToString("#####0"),
                                            TotalNumberPallets = dr1["PALLETS"].ToString(),
                                            TotalNumberPackages = (dr1["PALLETS"].ToString() == "0") ? dr1["PACKAGES"].ToString() : "0"
                                        };
                                        Edi.Measure = measure;
                                        pieces = dr1["PIECES"].ToString(); // Retrieves the total despatched qty per pallet from here, so there is no need to SUM or retrieve 2 from DB.
                                    }
                                }
                                //con1.Close();
                            }

                            switch (custID)
                            {
                                case "VWT":
                                case "DELP":
                                case "DELP_CN":
                                    Edi.Pallets = await GetPallets(shipPlanId, pieces, custID, packListID, Edi.ShipTo.ShipToID);
                                    break;
                                case "BRBE":
                                case "BRBS":
                                case "BRCA":
                                case "BRCG":
                                case "BRCH":
                                case "BRMX":
                                case "BRNB":
                                case "BRPB":
                                case "BRSJ":
                                case "BRSP":
                                    Edi.PartsV2 = await GetDetails(shipPlanId, pieces, custID, packListID, Edi.ShipTo.ShipToID);
                                    break;
                                default:
                                    break;
                            }

                            list.Add(Edi);
                        }
                    }
                    con.Close();
                }
            }
            catch (FormatException fex)
            {
                throw fex;
            }
            catch (DbException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        protected async Task<List<Pallet>> GetPallets(string shipPlanId, string pieces, string custID, string packListID, string shipToID)
        {
            List<Pallet> PalletList = new List<Pallet>();
            DataTable PalletTable = new DataTable();
            string querystr;

            if (custID == "VWT")
            {
                querystr = $@"WITH PALLETS(PALLET_SERIAL, PALLET_CODE, PALLET_DESCRIPTION, LID_CODE) 
                        AS(
                        SELECT SC.CONTAINER_ID [PALLET_SERIAL], 
                            CASE WHEN SC.WOOD_PALLET = 1 THEN '0007PAL' ELSE '484506' END AS [PALLET_CODE],
	                        CASE WHEN SC.RET_CONT = 1 THEN 'RETURNABLE' ELSE 'EXPENDABLE' END AS [PALLET_DESCRIPTION],
                            CASE WHEN SC.PLASTIC_PALLET = 1 THEN '500168' ELSE '       ' END AS [LID_CODE]
                        FROM SHIPPING_CONTAINERS SC 
                        WHERE VER_SHIP_PLANID = '{shipPlanId}'
                        AND LABEL_TYPE = 'MAS'
                        ),
                        BOXES(BOX_LABEL, BOX_CODE, BOX_DESCRIPTION, BOX_UOM, QUANTITY, QUANTITY_UOM, MANUFACTURE_DATE, LOT_NO, SKID_SERIAL_NO, fk_CUST_PART_NO, QTY_PER_BOX)
                        AS (
                        SELECT SC.CONTAINER_ID [BOX_LABEL],
	                        CMC.ITEM_NUMBER [BOX_CODE],
	                        CASE WHEN SC.RET_CONT = 1 THEN 'RETURNABLE' ELSE 'EXPENDABLE' END AS [BOX_DESCRIPTION],
                            CMC.ITEM_UOM [BOX_UOM],
                            SC.QTY [QUANTITY],
                            SC.Qty_UOM [QUANTITY_UOM],
	                        SC.DATE_PRINTED [MANUFACTURE_DATE],
	                        SC.LOT_NO [LOT_NO],
	                        SC.SKID_SERIAL_NO,
                            SC.fk_CUST_PART_NO,
                            PCK.PARTS_PER_CONTAINER [QTY_PER_BOX]
                        FROM SHIPPING_CONTAINERS SC JOIN PACKAGING PCK ON SC.Axiom_Part_No = PCK.fk_PART_NO,
                            (SELECT ITEM_ROW_ID, ITEM_NUMBER FROM PACK_LIST_ITEMS  WHERE FK_PACK_LIST_ID = '{packListID}' AND ITEM_TYPE = 'Part') PLI_ITEM,
                            (SELECT ITEM_ROW_ID, ITEM_NUMBER, ITEM_UOM FROM PACK_LIST_ITEMS  WHERE FK_PACK_LIST_ID = '{packListID}' AND PRIMARY_CONTAINER = 'Y' ) CMC
                        WHERE VER_SHIP_PLANID = '{shipPlanId}' AND QTY > 0
                        AND CONTAINER_TYPE <> 'SKID'
                        AND PLI_ITEM.ITEM_NUMBER = SC.fk_CUST_PART_NO
                        AND (PLI_ITEM.ITEM_ROW_ID+1) = CMC.ITEM_ROW_ID
                        AND PCK.fk_SHIP_TO_ID = '{shipToID}'
                        AND PCK.PKG_TYPE = (CASE WHEN SC.RET_CONT = 1 THEN 'RET' ELSE 'EXP'END)
                        ),
                        PARTS(ITEM_NUMBER, ITEM_DESCRIPTION, MANIFEST_NO, STORAGE_LOCATION, SA_NUM) 
                        AS(
                        SELECT PLI.ITEM_NUMBER, 
	                            PLI.ITEM_DESCRIPTION,
	                            DSPI.MANIFEST_NO,
                                DSPI.STORAGE_LOCATION,
                                DSPI.SA_NUM
                        FROM PACK_LIST_ITEMS PLI JOIN DAILY_SHIP_PLAN_ITEM DSPI ON PLI.ITEM_NUMBER = DSPI.PART_NUMBER
                        WHERE DSPI.fk_SHIP_PLANID = '{shipPlanId}' AND DSPI.SHIP_QTY > 0
                        AND PLI.FK_PACK_LIST_ID = '{packListID}'
                        )
                        SELECT * FROM PALLETS, BOXES, PARTS 
                        WHERE PALLETS.PALLET_SERIAL = BOXES.SKID_SERIAL_NO AND BOXES.fk_CUST_PART_NO = PARTS.ITEM_NUMBER
                        ORDER BY PARTS.ITEM_NUMBER, PALLET_SERIAL, QUANTITY desc;";
            }
            else
            {
                querystr = $@"WITH SKIDS(PALLET_SERIAL, PALLET_CODE, PALLET_DESCRIPTION) 
                        AS(
                        SELECT DISTINCT SC.CONTAINER_ID[PALLET_SERIAL],
                            PLI.ITEM_NUMBER[PALLET_CODE],
                            CASE WHEN SC.RET_CONT = 1 THEN 'RETURNABLE' ELSE 'EXPENDABLE' END AS[PALLET_DESCRIPTION]
                        FROM SHIPPING_CONTAINERS SC
                        LEFT JOIN PACK_LIST PL ON SC.fk_SHIP_PLANID = PL.fk_SHIP_PLANID
                        LEFT JOIN PACK_LIST_ITEMS PLI ON PL.PACK_LIST_ID = PLI.fk_PACK_LIST_ID
                        WHERE VER_SHIP_PLANID = '{shipPlanId}' and Label_Type = 'MAS'
                        AND PL.PACK_LIST_ID = '{packListID}' AND PLI.ITEM_TYPE like '%Skid' AND PLI.PALLETS = 1
                        ),
                        LIDS(PALLET_SERIAL, LID_CODE, LID_DESCRIPTION)
                        AS(
                        SELECT DISTINCT SC.CONTAINER_ID[PALLET_SERIAL],
                            PLI.ITEM_NUMBER[LID_CODE],
                            CASE WHEN SC.RET_CONT = 1 THEN 'RETURNABLE' ELSE 'EXPENDABLE' END AS[LID_DESCRIPTION]
                        FROM SHIPPING_CONTAINERS SC
                        LEFT JOIN PACK_LIST PL ON SC.fk_SHIP_PLANID = PL.fk_SHIP_PLANID
                        LEFT JOIN PACK_LIST_ITEMS PLI ON PL.PACK_LIST_ID = PLI.fk_PACK_LIST_ID
                        WHERE VER_SHIP_PLANID = '{shipPlanId}' and Label_Type = 'MAS'
                        AND PL.PACK_LIST_ID = '{packListID}' AND PLI.ITEM_TYPE like '%Lid'
                        ),
                        BOXES(BOX_LABEL, BOX_CODE, BOX_DESCRIPTION, BOX_UOM, QUANTITY, QUANTITY_UOM, MANUFACTURE_DATE, LOT_NO, SKID_SERIAL_NO, fk_CUST_PART_NO)
                        AS(
                        SELECT SC.CONTAINER_ID[BOX_LABEL],
                            CMC.ITEM_NUMBER[BOX_CODE],
                            CASE WHEN SC.RET_CONT = 1 THEN 'RETURNABLE' ELSE 'EXPENDABLE' END AS[BOX_DESCRIPTION],
                            CMC.ITEM_UOM[BOX_UOM],
                            SC.QTY[QUANTITY],
                            SC.Qty_UOM[QUANTITY_UOM],
                            SC.DATE_PRINTED[MANUFACTURE_DATE],
                            SC.LOT_NO[LOT_NO],
                            SC.SKID_SERIAL_NO,
                            SC.fk_CUST_PART_NO
                        FROM SHIPPING_CONTAINERS SC,
                            (SELECT ITEM_ROW_ID, ITEM_NUMBER FROM PACK_LIST_ITEMS  WHERE FK_PACK_LIST_ID = '{packListID}' AND ITEM_TYPE = 'Part') PLI_ITEM,
                            (SELECT ITEM_ROW_ID, ITEM_NUMBER, ITEM_UOM FROM PACK_LIST_ITEMS  WHERE FK_PACK_LIST_ID = '{packListID}' AND PRIMARY_CONTAINER = 'Y' ) CMC
                        WHERE VER_SHIP_PLANID = '{shipPlanId}' AND QTY > 0
                        AND CONTAINER_TYPE<> 'SKID'
                        AND PLI_ITEM.ITEM_NUMBER = SC.fk_CUST_PART_NO
                        AND(PLI_ITEM.ITEM_ROW_ID + 1) = CMC.ITEM_ROW_ID
                        ),
                        PARTS(ITEM_NUMBER, ITEM_DESCRIPTION, MANIFEST_NO, STORAGE_LOCATION, SA_NUM)
                        AS(
                        SELECT PLI.ITEM_NUMBER,
                                PLI.ITEM_DESCRIPTION,
                                DSPI.MANIFEST_NO,
                                DSPI.STORAGE_LOCATION,
                                DSPI.SA_NUM
                        FROM PACK_LIST_ITEMS PLI JOIN DAILY_SHIP_PLAN_ITEM DSPI ON PLI.ITEM_NUMBER = DSPI.PART_NUMBER
                        WHERE DSPI.fk_SHIP_PLANID = '{shipPlanId}' AND DSPI.SHIP_QTY > 0
                        AND PLI.FK_PACK_LIST_ID = '{packListID}'
                        )
                        SELECT *
                        FROM SKIDS LEFT JOIN LIDS ON SKIDS.PALLET_SERIAL = LIDS.PALLET_SERIAL , BOXES, PARTS
                        WHERE SKIDS.PALLET_SERIAL = BOXES.SKID_SERIAL_NO AND BOXES.fk_CUST_PART_NO = PARTS.ITEM_NUMBER
                        ORDER BY PARTS.ITEM_NUMBER, SKIDS.PALLET_SERIAL;";
            }

            try
            {
                // ToDo: Add condition for selecting Class according to query
                var con = (SqlConnection)_context.Database.GetDbConnection();
                var table = con.Query<PalletTable>(querystr).ToList();

                Pallet pallet = new Pallet();
                string palletSerialNumber;
                DataTableReader dr = new DataTableReader(PalletTable);
                int boxesSum = 0, boxQty = 0, partsSum = 0, palletIndex = 1;

                if (table.Count > 0)
                {
                    foreach (var row in table)
                    {
                        palletSerialNumber = row.Pallet_Serial;
                        if (pallet.PalletSerialNumber != palletSerialNumber) // If the current pallet from record is not equal to the one in memory, a new Pallet instance must be created.
                        {
                            if (pallet.PalletSerialNumber != null) //The first pallet instance needs to have initial data...
                            {
                                pallet.NumberOfBoxes = boxQty.ToString();
                                boxQty = 0;
                                pallet.NumberOfBoxesUOM = row.Box_UOM;
                                pallet.NumberOfParts = partsSum.ToString();
                                pallet.NumberOfPartsUOM = row.Quantity_UOM;
                                pallet.PartTotalDespatchedQty = pieces.ToString();
                                PalletList.Add(pallet);
                                pallet = new Pallet();
                                partsSum = 0;
                            }
                            pallet.PalletID = palletIndex.ToString();
                            pallet.PalletSerialNumber = row.Pallet_Serial;
                            switch (custID)
                            {
                                case "VWT":
                                    pallet.PalletLabelNumber = "6JUN259898559" + row.Pallet_Serial;
                                    pallet.NumberOfPartsPerBox = row.Quantity;
                                    break;
                                default:
                                    pallet.NumberOfPartsPerBox = row.Quantity;
                                    pallet.PalletLabelNumber = row.Pallet_Serial;
                                    break;
                            }
                            pallet.PalletCode = row.Pallet_Code;
                            pallet.PalletDescription = ParseText(row.Pallet_Description);
                            pallet.LidCode = row.Lid_Code;
                            pallet.LidDescription = (row.Lid_Code.Trim() != "") ? "Top Cap" : "";
                            //pallet.LidQuantity = (row.Lid_Description == "") ? "" : "1";
                            pallet.BoxMaterialCode = row.Box_Code.ToString();
                            pallet.BoxDescription = row.Box_Description.ToString();
                            pallet.BuyerPartNumber = row.Item_Number.ToString();
                            pallet.PartDescription = ParseText(row.Item_Description.ToString());
                            pallet.ManifestNumber = row.Manifest_No.ToString();
                            pallet.CountryOrigin = "CA"; // HARDCODED -- NO INFORMATION IN TABLES, TAKING COUNTRY FROM LABEL TEMPLATE.
                            pallet.StorageLocation = row.Storage_Location.ToString();
                            pallet.SchedulingAgreementNo = row.Sa_Num.ToString();
                            palletIndex++;
                        }

                        boxesSum++;
                        boxQty++;
                        Box box = new Box();
                        box.BoxID = boxesSum.ToString("00000");
                        box.BoxSerialNumber = row.Box_Label.ToString();
                        box.BoxLabelNumber = (custID == "VWT") ? ("1JUN259898559" + row.Box_Label.ToString()) : row.Box_Label.ToString();
                        box.ManufactureDate = DateTime.Parse(row.Manufacture_Date.ToString()).ToString("yyyyMMdd");
                        box.LotNumber = row.Lot_No.ToString();
                        box.QuantityPerBox = row.Quantity.ToString();

                        partsSum += Int32.Parse(row.Quantity.ToString());
                        pallet.Boxes.Add(box);
                    }
                    // Finally, assign the values of boxs and parts to the last pallet in list.
                    pallet.NumberOfBoxes = boxQty.ToString();
                    pallet.NumberOfBoxesUOM = table[table.Count - 1].Box_UOM.ToString();
                    pallet.NumberOfParts = partsSum.ToString();
                    pallet.NumberOfPartsUOM = table[table.Count - 1].Quantity_UOM.ToString();
                    pallet.PartTotalDespatchedQty = pieces.ToString();
                    PalletList.Add(pallet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PalletList;
        }

        protected async Task<List<PartV2>> GetDetails(string shipPlanId, string pieces, string custID, string packListID, string shipToID)
        {
            List<PartV2> DetailList = new List<PartV2>();
            DataTable DetailTable = new DataTable();
            try
            {
                //using (DbConnection con = _context.Database.GetDbConnection())
                var con = (SqlConnection)_context.Database.GetDbConnection();
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = $@"WITH SKIDS(fk_CUST_PART_NO, PALLET_SERIAL, AXIOM_PALLET_CODE, AXIOM_PALLET_DESCRIPTION, PALLET_CODE, PALLET_DESCRIPTION, SKID_PKG_TYPE) -- PALLET_CODE, PALLET_DESCRIPTION,
                                        AS(
                                        SELECT DISTINCT fk_CUST_PART_NO, 
		                                        skid_serial_no [PALLET_SERIAL], 
                                                PLI.fk_Part_Num [AXIOM_PALLET_CODE],
                                                P.PART_DESCRIPTION [AXIOM_PALLET_DESCRIPTION],
		                                        PLI.ITEM_NUMBER [PALLET_CODE], 
		                                        PLI.ITEM_DESCRIPTION [PALLET_DESCRIPTION], 
		                                        CASE WHEN SC.RET_CONT = 1 THEN 'Returnable' ELSE 'Expendable' END AS SKID_PKG_TYPE
                                        FROM SHIPPING_CONTAINERS SC, PACK_LIST_ITEMS PLI LEFT JOIN PART P ON P.PART_NO = PLI.fk_Part_Num
                                        WHERE SC.fk_SHIP_PLANID = '{shipPlanId}' AND SC.QTY > 0 AND (SC.CONTAINER_TYPE <> 'SKID' OR SC.CONTAINER_TYPE IS NULL)
                                        AND PLI.fk_PACK_LIST_ID = '{packListID}' AND PLI.ITEM_TYPE like '%Skid' AND PLI.PALLETS = 1
                                        AND PLI.RETURN_CONTAINER = (CASE WHEN SC.RET_CONT = 1 THEN 'Y' ELSE 'N'END)
                                        AND PLI.fk_Part_Num in (SELECT fk_PART_NO from PACKAGING_ITEMS where ASN_CODE = PLI.ITEM_NUMBER
                                        AND fk_PKG_ID = (SELECT PKG_ID FROM PACKAGING WHERE fk_SHIP_TO_ID = '{shipToID}' AND fk_PART_NO = SC.Axiom_Part_No AND PKG_TYPE = CASE WHEN SC.RET_CONT = 1 THEN 'RET' ELSE 'EXP' END))
                                        ),
                                        LIDS(fk_CUST_PART_NO, PALLET_SERIAL_LID, AXIOM_LID_CODE, AXIOM_LID_DESCRIPTION, LID_PKG_TYPE, RETURN_CONTAINER, LID_CODE, LID_DESCRIPTION) -- LID_CODE, LID_DESCRIPTION
                                        AS(
                                        SELECT	DISTINCT fk_CUST_PART_NO, 
		                                        skid_serial_no [PALLET_SERIAL_LID],
		                                        PLI.fk_Part_Num [AXIOM_LID_CODE],
		                                        P.PART_DESCRIPTION [AXIOM_LID_DESCRIPTION],
		                                        CASE WHEN RET_CONT = 1 THEN 'Returnable' ELSE 'Expendable' END AS LID_PKG_TYPE,
		                                        PLI.RETURN_CONTAINER, 
		                                        PLI.ITEM_NUMBER [LID_CODE], 
		                                        PLI.ITEM_DESCRIPTION [LID_DESCRIPTION]
                                        FROM SHIPPING_CONTAINERS SC
                                        LEFT OUTER JOIN PACK_LIST PL ON SC.fk_SHIP_PLANID = PL.fk_SHIP_PLANID
                                        LEFT OUTER JOIN PACK_LIST_ITEMS PLI ON PL.PACK_LIST_ID = PLI.fk_PACK_LIST_ID
                                        LEFT JOIN PART P ON P.PART_NO = PLI.fk_Part_Num
                                        WHERE SC.fk_SHIP_PLANID = '{shipPlanId}' AND QTY > 0 AND (CONTAINER_TYPE <> 'SKID' OR CONTAINER_TYPE IS NULL) 
                                        AND PL.PACK_LIST_ID = '{packListID}' AND PLI.ITEM_TYPE like '%Lid' AND (PLI.PALLETS = 0 OR PLI.PALLETS IS NULL)
                                        AND PLI.RETURN_CONTAINER = (CASE WHEN SC.RET_CONT = 1 THEN 'Y' ELSE 'N'END)
                                        ),
                                        BOXES(BOX_LABEL, AXIOM_BOX_CODE, AXIOM_BOX_DESCRIPTION, BOX_CODE, BOX_TYPE, BOX_DESCRIPTION, BOX_UOM, QUANTITY, QUANTITY_UOM, MANUFACTURE_DATE, LOT_NO, SKID_SERIAL_NO, fk_CUST_PART_NO) -- BOX_CODE, BOX_DESCRIPTION, BOX_UOM
                                        AS (
                                        SELECT SC.CONTAINER_ID [BOX_LABEL],
                                            CMC.fk_Part_Num [AXIOM_BOX_CODE],
	                                        P.PART_DESCRIPTION [AXIOM_BOX_DESCRIPTION],
	                                        CMC.ITEM_NUMBER [BOX_CODE],
	                                        CASE WHEN SC.RET_CONT = 1 THEN 'Returnable' ELSE 'Expendable' END AS [BOX_TYPE],
	                                        CMC.ITEM_DESCRIPTION [BOX_DESCRIPTION],
                                            CMC.ITEM_UOM [BOX_UOM],
                                            SC.QTY [QUANTITY],
                                            SC.Qty_UOM [QUANTITY_UOM],
	                                        SC.DATE_PRINTED [MANUFACTURE_DATE],
	                                        SC.LOT_NO [LOT_NO],
	                                        SC.SKID_SERIAL_NO,
                                            SC.fk_CUST_PART_NO
                                        FROM SHIPPING_CONTAINERS SC,
                                            (SELECT ITEM_ROW_ID, ITEM_NUMBER FROM PACK_LIST_ITEMS  WHERE FK_PACK_LIST_ID = '{packListID}' AND ITEM_TYPE = 'Part') PLI_ITEM,
                                            (SELECT ITEM_ROW_ID, ITEM_NUMBER, ITEM_DESCRIPTION, ITEM_UOM, RETURN_CONTAINER, fk_Part_Num FROM PACK_LIST_ITEMS  WHERE FK_PACK_LIST_ID = '{packListID}' AND PRIMARY_CONTAINER = 'Y' ) CMC
                                            LEFT JOIN PART P ON P.PART_NO = CMC.fk_Part_Num
                                        WHERE VER_SHIP_PLANID = '{shipPlanId}' AND QTY > 0
                                        AND CONTAINER_TYPE <> 'SKID'
                                        AND PLI_ITEM.ITEM_NUMBER = SC.fk_CUST_PART_NO
                                        AND (PLI_ITEM.ITEM_ROW_ID+1) = CMC.ITEM_ROW_ID
                                        AND CMC.RETURN_CONTAINER = (CASE WHEN SC.RET_CONT = 1 THEN 'Y' ELSE 'N'END)
                                        ),
                                        PARTS(ITEM_NUMBER, ITEM_DESCRIPTION, fk_Part_Num, PART_DESCRIPTION, PO_NUMBER, MISC, STORAGE_LOCATION, SA_NUM, DOCK_CODE)
                                        AS(
                                        SELECT distinct PLI.ITEM_NUMBER, 
	                                            PLI.ITEM_DESCRIPTION,
	                                            PLI.fk_Part_Num,
	                                            P.PART_DESCRIPTION,
	                                            PLI.PO_NUMBER,
	                                            CASE WHEN PLI.MISC_DESC = 'PO Line' THEN PLI.MISC ELSE '' END [MISC],
	                                            DSPI.STORAGE_LOCATION,
	                                            CASE WHEN PLI.MISC_DESC = 'SA' THEN PLI.MISC ELSE DSPI.SA_NUM END [SA_NUM],
	                                            PP.DOCK_CODE
                                        FROM PACK_LIST_ITEMS PLI 
                                        LEFT JOIN DAILY_SHIP_PLAN_ITEM DSPI ON PLI.ITEM_NUMBER = DSPI.PART_NUMBER
                                        LEFT JOIN PART P ON PLI.fk_Part_Num = P.PART_NO
                                        LEFT JOIN PART_PLANT PP ON P.PART_NO = PP.fk_PART_NO
                                        WHERE DSPI.fk_SHIP_PLANID = '{shipPlanId}' AND DSPI.SHIP_QTY > 0
                                        AND PLI.FK_PACK_LIST_ID = '{packListID}'
                                        AND PP.fk_SHIP_TO_ID = '{shipToID}'
                                        )
                                        SELECT ITEM_NUMBER, ITEM_DESCRIPTION, FK_PART_NUM, PART_DESCRIPTION, PO_NUMBER, MISC, STORAGE_LOCATION, SA_NUM, DOCK_CODE, 
	                                            SKIDS.PALLET_SERIAL, SKIDS.PALLET_CODE, SKIDS.PALLET_DESCRIPTION, SKIDS.SKID_PKG_TYPE, SKIDS.AXIOM_PALLET_CODE, SKIDS.AXIOM_PALLET_DESCRIPTION,
	                                            LIDS.PALLET_SERIAL_LID, LIDS.LID_CODE, LIDS.LID_DESCRIPTION, LIDS.LID_PKG_TYPE, LIDS.RETURN_CONTAINER, LIDS.AXIOM_LID_CODE, LIDS.AXIOM_LID_DESCRIPTION,
	                                            BOX_LABEL, AXIOM_BOX_CODE, AXIOM_BOX_DESCRIPTION, BOX_CODE, BOX_DESCRIPTION, BOX_TYPE, BOX_UOM, QUANTITY, QUANTITY_UOM, MANUFACTURE_DATE, LOT_NO
                                        FROM SKIDS LEFT JOIN LIDS ON SKIDS.PALLET_SERIAL = LIDS.PALLET_SERIAL_LID, BOXES, PARTS 
                                        WHERE SKIDS.PALLET_SERIAL = BOXES.SKID_SERIAL_NO AND BOXES.fk_CUST_PART_NO = PARTS.ITEM_NUMBER
                                        ORDER BY PARTS.ITEM_NUMBER, SKIDS.PALLET_SERIAL;";
                    cmd.Connection = con;
                    await con.OpenAsync();
                    DbDataAdapter DataAdapter = null;
                    DataAdapter.SelectCommand = cmd;
                    DataAdapter.Fill(DetailTable);
                    //con.Close();
                }

                if (DetailTable.Rows.Count > 0)
                {
                    PartV2 part = new PartV2();
                    string partNum;
                    DataTableReader dr = new DataTableReader(DetailTable);
                    PalletV2 pallet = new PalletV2();
                    List<PalletV2> palletList = new List<PalletV2>();
                    string palletSerialNumber;
                    int boxesSum = 0, boxQty = 0, partsSum = 0, palletIndex = 1, PartTotalDespatchedQty = 0;

                    while (dr.Read())
                    {
                        partNum = dr["ITEM_NUMBER"].ToString();
                        // First check the part number must be the same in order to process next pallet and box.
                        if (part.PartNumber != partNum)
                        {
                            if (part.PartNumber != null)
                            {
                                PartTotalDespatchedQty += partsSum;
                                part.PartTotalDespatchedQty = PartTotalDespatchedQty.ToString();
                                pallet.NumberOfBoxes = boxQty.ToString();
                                pallet.NumberOfBoxesUOM = dr["BOX_UOM"].ToString();
                                pallet.NumberOfParts = partsSum.ToString();
                                pallet.NumberOfPartsUOM = dr["QUANTITY_UOM"].ToString();
                                palletList.Add(pallet);
                                pallet = new PalletV2();
                                part.PalletV2 = palletList;
                                palletList = new List<PalletV2>();
                                DetailList.Add(part);
                                part = new PartV2();
                                partsSum = 0;
                                boxQty = 0;
                                PartTotalDespatchedQty = 0;
                            }
                            part.PartNumber = partNum;
                            part.PartDescription = ParseText(dr["ITEM_DESCRIPTION"].ToString());
                            part.AxiomPartNum = dr["fk_Part_Num"].ToString();
                            part.AxiomPartDescription = ParseText(dr["PART_DESCRIPTION"].ToString());
                            part.PoNumber = dr["PO_NUMBER"].ToString();
                            part.PoLineNumber = dr["MISC"].ToString();
                            part.CountryOrigin = "CA"; // HARDCODED -- NO INFORMATION IN TABLES, TAKING COUNTRY FROM LABEL TEMPLATE.
                            part.SchedulingAgreementNo = dr["SA_NUM"].ToString();
                            part.ReleaseNumber = "";//dr["STORAGE_LOCATION"].ToString();
                            part.DockCode = dr["DOCK_CODE"].ToString();
                        }

                        palletSerialNumber = dr["PALLET_SERIAL"].ToString();
                        if (pallet.PalletSerialNumber != palletSerialNumber) // If the current pallet from record is not equal to the one in memory, a new Pallet instance must be created.
                        {
                            if (pallet.PalletSerialNumber != null) //The first pallet instance needs to have initial data...
                            {
                                PartTotalDespatchedQty += partsSum;
                                pallet.NumberOfBoxes = boxQty.ToString();
                                pallet.NumberOfBoxesUOM = dr["BOX_UOM"].ToString();
                                pallet.NumberOfParts = partsSum.ToString();
                                pallet.NumberOfPartsUOM = dr["QUANTITY_UOM"].ToString();
                                palletList.Add(pallet);
                                pallet = new PalletV2();
                                partsSum = 0;
                                boxQty = 0;
                            }
                            pallet.PalletID = palletIndex.ToString();
                            pallet.PalletSerialNumber = dr["PALLET_SERIAL"].ToString();
                            switch (custID)
                            {
                                case "VWT":
                                    pallet.PalletLabelNumber = "6JUN259898559" + pallet.PalletSerialNumber;
                                    break;
                                default:
                                    pallet.PalletLabelNumber = pallet.PalletSerialNumber;
                                    break;
                            }
                            pallet.PalletCode = dr["AXIOM_PALLET_CODE"].ToString();
                            pallet.PalletDescription = ParseText(dr["AXIOM_PALLET_DESCRIPTION"].ToString());
                            pallet.CustomerPalletCode = dr["PALLET_CODE"].ToString();
                            pallet.CustomerPalletDescription = ParseText(dr["PALLET_DESCRIPTION"].ToString());
                            pallet.PalletType = dr["SKID_PKG_TYPE"].ToString();
                            pallet.LidCode = dr["AXIOM_LID_CODE"].ToString();
                            pallet.LidDescription = ParseText(dr["AXIOM_LID_DESCRIPTION"].ToString());
                            pallet.CustomerLidCode = dr["LID_CODE"].ToString();
                            pallet.CustomerLidDescription = ParseText(dr["LID_DESCRIPTION"].ToString());
                            pallet.LidType = dr["LID_PKG_TYPE"].ToString();
                            //pallet.LidQuantity = (pallet.LidType == "") ? "" : "1";
                            pallet.BoxMaterialCode = dr["AXIOM_BOX_CODE"].ToString();
                            pallet.BoxDescription = ParseText(dr["AXIOM_BOX_DESCRIPTION"].ToString());
                            pallet.CustomerBoxCode = dr["BOX_CODE"].ToString();
                            pallet.CustomerBoxDescription = ParseText(dr["BOX_DESCRIPTION"].ToString());
                            pallet.BoxType = dr["BOX_TYPE"].ToString();
                            pallet.NumberOfPartsPerBox = dr["QUANTITY"].ToString();
                            palletIndex++;
                        }

                        boxesSum++;
                        boxQty++;
                        Box box = new Box();
                        box.BoxID = boxesSum.ToString("00000");
                        box.BoxSerialNumber = dr["BOX_LABEL"].ToString();
                        box.BoxLabelNumber = (custID == "VWT") ? ("1JUN259898559" + dr["BOX_LABEL"].ToString()) : dr["BOX_LABEL"].ToString();
                        box.ManufactureDate = DateTime.Parse(dr["MANUFACTURE_DATE"].ToString()).ToString("yyyyMMdd");
                        box.LotNumber = dr["LOT_NO"].ToString();

                        partsSum += Int32.Parse(pallet.NumberOfPartsPerBox);
                        pallet.Boxes.Add(box);

                    }
                    PartTotalDespatchedQty += partsSum;
                    part.PartTotalDespatchedQty = PartTotalDespatchedQty.ToString();
                    pallet.NumberOfBoxes = boxQty.ToString();
                    pallet.NumberOfBoxesUOM = dr["BOX_UOM"].ToString();
                    pallet.NumberOfParts = partsSum.ToString();
                    pallet.NumberOfPartsUOM = dr["QUANTITY_UOM"].ToString();
                    palletList.Add(pallet);
                    part.PalletV2 = palletList;
                    DetailList.Add(part);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return DetailList;
        }

        protected static string CreateLines(List<Edifact> EdiList, MapInfo map)
        {
            // It will contain how many characters each field should use in the line.
            StringBuilder outputLine = new StringBuilder();
            var info = typeof(Edifact).GetProperties();

            outputLine.Append("HDR");
            outputLine.Append(AppendInformation(map.Header.PartnerID, 9));
            outputLine.Append(AppendInformation(map.Header.Standard, 1));
            outputLine.Append(AppendInformation(map.Header.Version, 6));
            outputLine.Append(AppendInformation(map.Header.TransactionSet, 6));
            outputLine.Append(AppendInformation(map.Header.TestProd, 1));
            outputLine.AppendLine();

            outputLine.Append(AppendInformation("HEADER", 6));
            foreach (Edifact edi in EdiList)
            {
                outputLine.Append(ReadStructure(edi, info, map.Position));
            }
            outputLine.AppendLine();
            return outputLine.ToString();
        }

        // It will be use recursively, each structure will go inside and check for Strings to make the print of values
        // if not, it will loop until it finds a string in the structure.
        protected static string ReadStructure(object callerObj, System.Reflection.PropertyInfo[] propArray, Dictionary<string, int> position)
        {
            PropertyInfo propertyInfo = null;
            object obj = null;
            StringBuilder outputLine = new StringBuilder();
            PropertyInfo[] info = null;
            try
            {
                for (int i = 0; i < propArray.Length; i++)
                {
                    if ((!position.ContainsKey(propArray[i].Name)) && propArray[i].PropertyType.Name == "String")
                    {
                        continue;
                    }
                    propertyInfo = callerObj.GetType().GetProperty(propArray[i].Name.ToString());
                    obj = propertyInfo.GetValue(callerObj, null);
                    obj = (obj == null) ? "" : obj;
                    switch (propArray[i].PropertyType.Name)
                    {
                        case "String":
                            outputLine.Append(AppendInformation(obj.ToString(), position[propArray[i].Name]));
                            break;
                        case "DocHeader":
                            info = typeof(DocHeader).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "SoldTo":
                            outputLine.Append(AppendInformation("SOLD_TO", 7));
                            info = typeof(SoldTo).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "ShipTo":
                            outputLine.Append(AppendInformation("SHIP_TO", 7));
                            info = typeof(ShipTo).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "Seller":
                            outputLine.Append(AppendInformation("SELLER", 6));
                            info = typeof(Seller).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "ShipFrom":
                            outputLine.Append(AppendInformation("SHIP_FR", 7));
                            info = typeof(ShipFrom).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "Freight":
                            outputLine.Append(AppendInformation("FREIGHT", 7));
                            info = typeof(Freight).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "Measure":
                            outputLine.Append(AppendInformation("MEASURE", 7));
                            info = typeof(Measure).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            break;
                        case "ShipDates":
                            outputLine.Append(AppendInformation("SHIP_DATE", 9));
                            info = typeof(ShipDates).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "ReqDates":
                            outputLine.Append(AppendInformation("REQ_DEL_DATE", 12));
                            info = typeof(ReqDates).GetProperties();
                            outputLine.Append(ReadStructure(obj, info, position));
                            outputLine.AppendLine();
                            break;
                        case "List`1":
                            if (obj.ToString() == "")
                            {
                                continue;
                            }
                            string fullName = propArray[i].PropertyType.FullName;
                            fullName = fullName.Substring(fullName.IndexOf("[[") + 2, fullName.IndexOf(",") - (fullName.IndexOf("[[") + 2)).Replace("IOEdiProcessor.Data.", "");
                            switch (fullName)
                            {
                                case "Pallet":
                                    info = typeof(Pallet).GetProperties();
                                    foreach (object o in (List<Pallet>)obj)
                                    {
                                        outputLine.AppendLine();
                                        outputLine.Append(AppendInformation("PALLET", 6));
                                        outputLine.Append(ReadStructure(o, info, position));
                                    }
                                    break;
                                case "Box":
                                    info = typeof(Box).GetProperties();
                                    foreach (object o in (List<Box>)obj)
                                    {
                                        outputLine.AppendLine();
                                        outputLine.Append(AppendInformation("BOX", 3));
                                        outputLine.Append(ReadStructure(o, info, position));
                                    }
                                    break;
                                case "Part":
                                    info = typeof(Part).GetProperties();
                                    foreach (object o in (List<Part>)obj)
                                    {
                                        outputLine.AppendLine();
                                        outputLine.Append(AppendInformation("PART", 4));
                                        outputLine.Append(ReadStructure(o, info, position));
                                    }
                                    break;
                                case "PartV2":
                                    info = typeof(PartV2).GetProperties();
                                    foreach (object o in (List<PartV2>)obj)
                                    {
                                        outputLine.AppendLine();
                                        outputLine.Append(AppendInformation("PART", 4));
                                        outputLine.Append(ReadStructure(o, info, position));
                                    }
                                    break;
                                case "PalletV2":
                                    info = typeof(PalletV2).GetProperties();
                                    foreach (object o in (List<PalletV2>)obj)
                                    {
                                        outputLine.AppendLine();
                                        outputLine.Append(AppendInformation("PALLET", 6));
                                        outputLine.Append(ReadStructure(o, info, position));
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return outputLine.ToString();
        }

        protected static string AppendInformation(string str, int infoLength)
        {
            if (str == null)
            {
                str = "";
            }
            StringBuilder sb = new StringBuilder();
            str = str.Replace("\r\n", " ").Trim();
            if (str.Length > infoLength)
            {
                sb.Append(str, 0, infoLength);
            }
            else
            {
                sb.Append(str);
                for (int i = str.Length - 1; i < infoLength - 1; i++)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        protected static string ParseText(string str)
        {
            foreach (KeyValuePair<string, string> entry in _parse)
            {
                str = str.Replace(entry.Key, entry.Value);
            }
            return str;
        }
        
        // Old ASN functions

    }
}
