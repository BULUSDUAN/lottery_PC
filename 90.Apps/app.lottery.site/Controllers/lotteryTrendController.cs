using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace app.lottery.site.Controllers
{
    public class lotteryTrendController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 双色球走势
        /// </summary>
        public ActionResult SSQ(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_JiBenZouSi_Info(pageSize);
                        break;
                    case "daxiao":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_DX_Info(pageSize);
                        break;
                    case "chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_C3_Info(pageSize);
                        break;
                    case "hezhi":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_HeZhi_Info(pageSize);
                        break;
                    case "jiou":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_JiOu_Info(pageSize);
                        break;
                    case "kuadu_12":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_1_6_Info(pageSize);
                        break;
                    case "kuadu_23":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_1_6_Info(pageSize);
                        break;
                    case "kuadu_34":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_1_6_Info(pageSize);
                        break;
                    case "kuadu_45":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_1_6_Info(pageSize);
                        break;
                    case "kuadu_56":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_1_6_Info(pageSize);
                        break;
                    case "kuadu_sw":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_KuaDu_SW_Info(pageSize);
                        break;
                    case "zhihe":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SSQ_ZhiHe_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }

            return View("SSQ/" + id);
        }

        /// <summary>
        /// 大乐透走势
        /// </summary>
        public ActionResult DLT(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_JiBenZouSi_Info(pageSize);
                        break;
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_DX_Info(pageSize);
                        break;
                    case "jiou":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_JiOu_Info(pageSize);
                        break;
                    case "zhihe":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_ZhiHe_Info(pageSize);
                        break;
                    case "hezhi":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_HeZhi_Info(pageSize);
                        break;
                    case "chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_Chu3_Info(pageSize);
                        break;
                    case "kuadu_sw":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_KuaDu_SW_Info(pageSize);
                        break;
                    case "kuadu_12":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_KuaDu_12_Info(pageSize);
                        break;
                    case "kuadu_23":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_KuaDu_23_Info(pageSize);
                        break;
                    case "kuadu_34":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_KuaDu_34_Info(pageSize);
                        break;
                    case "kuadu_45":
                        ViewBag.DataList = WCFClients.ChartClient.QueryDLT_KuaDu_45_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("DLT/" + id);
        }

        /// <summary>
        /// 排列3
        /// </summary>
        public ActionResult PL3(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_JiBenZouSi_Info(pageSize);
                        break;
                    case "zxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_ZuXuanZouSi_info(pageSize);
                        break;
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_DX_info(pageSize);
                        break;
                    case "dxhm":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_DXHM_info(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_JIOU_info(pageSize);
                        break;
                    case "johm":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_JOHM_info(pageSize);
                        break;
                    case "zhihe":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_ZhiHe_info(pageSize);
                        break;
                    case "zhhm":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_ZHHM_info(pageSize);
                        break;
                    case "hezhi":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_HeiZhi_Info(pageSize);
                        break;
                    case "hztz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_PL3_HZTZ_Info(pageSize);
                        break;
                    case "hzhw":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_PL3_HZHW_Info(pageSize);
                        break;
                    case "kdbs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_KuaDu_12_Info(pageSize);
                        break;
                    case "kdsg":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_KuaDu_23_Info(pageSize);
                        break;
                    case "kdbg":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_KuaDu_13_Info(pageSize);
                        break;
                    case "chu33":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_PL3_Chu32_Info(pageSize);
                        break;
                    case "chu32":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_PL3_Chu31_Info(pageSize);
                        break;
                    case "chu31":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL3_PL3_Chu33_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("pl3/" + id);
        }

        /// <summary>
        /// 福彩3D
        /// </summary>
        public ActionResult FC3D(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "zxzs" : id;
                switch (id)
                {
                    case "zxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_ZhiXuanZouSi_Info(pageSize);
                        break;
                    case "zuxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_ZuXuanZouSi_Info(pageSize);
                        break;
                    case "dxxt":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_DXXT_Info(pageSize);
                        break;
                    case "dxhm":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_DXHM_Info(pageSize);
                        break;
                    case "joxt":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_JOXT_Info(pageSize);
                        break;
                    case "johm":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_JOHM_Info(pageSize);
                        break;
                    case "zhxt":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_ZHXT_Info(pageSize);
                        break;
                    case "zhhm":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_ZHHM_Info(pageSize);
                        break;
                    case "chu33":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_Chu32_Info(pageSize);
                        break;
                    case "chu32":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_Chu31_Info(pageSize);
                        break;
                    case "chu31":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_Chu33_Info(pageSize);
                        break;
                    case "hzfb":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_HZFB_Info(pageSize);
                        break;
                    case "hztz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_HZTZ_Info(pageSize);
                        break;
                    case "hwzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_HZZS_Info(pageSize);
                        break;
                    case "kuadu13":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_13_Info(pageSize);
                        break;
                    case "kuadu12":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_12_Info(pageSize);
                        break;
                    case "kuadu23":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_23_Info(pageSize);
                        break;
                    case "kuadu":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_FC3D_KuaDu_Z_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }

            return View("fc3d/" + id);
        }

        /// <summary>
        /// 江西11选5
        /// </summary>
        public ActionResult JX11X5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "rxjbzs" : id;
                switch (id)
                {
                    case "rxjbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RXJBZS_Info(pageSize);
                        break;
                    case "rxdx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RXDX_Info(pageSize);
                        break;
                    case "rxjo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RXJO_Info(pageSize);
                        break;
                    case "rxzh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RXZH_Info(pageSize);
                        break;
                    case "rxhz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RXHZ_Info(pageSize);
                        break;
                    case "rxchu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Chu3_Info(pageSize);
                        break;
                    case "rx1":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RX1_Info(pageSize);
                        break;
                    case "rx2":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RX2_Info(pageSize);
                        break;
                    case "rx3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RX3_Info(pageSize);
                        break;
                    case "rx4":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RX4_Info(pageSize);
                        break;
                    case "rx5":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_RX5_Info(pageSize);
                        break;
                    case "q3zx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3ZS_Info(pageSize);
                        break;
                    case "q3zux":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3ZUS_Info(pageSize);
                        break;
                    case "q3dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3DX_Info(pageSize);
                        break;
                    case "q3jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3JO_Info(pageSize);
                        break;
                    case "q3zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3ZH_Info(pageSize);
                        break;
                    case "q3chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3Chu3_Info(pageSize);
                        break;
                    case "q3hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q3HZ_Info(pageSize);
                        break;
                    case "q2zx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q2ZS_Info(pageSize);
                        break;
                    case "q2zux":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q2ZUS_Info(pageSize);
                        break;
                    case "q2hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJX11X5_Q2HZ_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("jx11x5/" + id);
        }


        /// <summary>
        /// 重庆时时彩
        /// </summary>
        public ActionResult CQSSC(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                var phaseOrder = string.IsNullOrEmpty(Request["phaseOrder"]) ? "down" : Request["phaseOrder"];
                ViewBag.PhasseOrder = phaseOrder;
                id = string.IsNullOrEmpty(id) ? "s3zxzs" : id;
                switch (id)
                {
                    case "s2hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_2X_HZZS_Info(pageSize);
                        break;
                    case "s2zuxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_2X_ZuXZS_Info(pageSize);
                        break;
                    case "s2zxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_2X_ZXZS_Info(pageSize);
                        break;
                    case "s3c3ys":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_C3YS_Info(pageSize);
                        break;
                    case "s3dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_DXZS_Info(pageSize);
                        break;
                    case "s3hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_HZZS_Info(pageSize);
                        break;
                    case "s3jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_JOZS_Info(pageSize);
                        break;
                    case "s3kd":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_KD_Info(pageSize);
                        break;
                    case "s3zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZHZS_Info(pageSize);
                        break;
                    case "s3zuxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZuXZS_Info(pageSize);
                        break;
                    case "s3zxzs":
                        //ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZXZS_Info_New(pageSize, phaseOrder);
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_3X_ZXZS_Info(pageSize);
                        break;
                    case "s5hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_5X_HZZS_Info(pageSize);
                        break;
                    case "s5jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_5X_JBZS_Info(pageSize);
                        break;
                    case "dxds":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_DXDS_Info(pageSize);
                        break;
                    case "s1zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQSSC_1X_ZS_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("cqssc/" + id);
        }

        /// <summary>
        /// 重庆11X5
        /// </summary>
        public ActionResult CQ11X5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_JBZS_Info(pageSize);
                        break;
                    case "chzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_CHZS_Info(pageSize);
                        break;
                    case "dlzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_DLZS_Info(pageSize);
                        break;
                    case "dwzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_012DWZS_Info(pageSize);
                        break;
                    case "elzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_2LZS_Info(pageSize);
                        break;
                    case "ghzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_GHZS_Info(pageSize);
                        break;
                    case "hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_HZZS_Info(pageSize);
                        break;
                    case "kdzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_KDZS_Info(pageSize);
                        break;
                    case "lzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_012LZZS_Info(pageSize);
                        break;
                    case "q1jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_Q1JBZS_Info(pageSize);
                        break;
                    case "q1xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_Q1XTZS_Info(pageSize);
                        break;
                    case "q2jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_Q2JBZS_Info(pageSize);
                        break;
                    case "q2xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_Q2XTZS_Info(pageSize);
                        break;
                    case "q3jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_Q3JBZS_Info(pageSize);
                        break;
                    case "q3xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_Q3XTZS_Info(pageSize);
                        break;
                    case "xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQ11X5_XTZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("cq11x5/" + id);
        }

        /// <summary>
        /// 广东11X5
        /// </summary>
        public ActionResult GD11X5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_JBZS_Info(pageSize);
                        break;
                    case "chzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_CHZS_Info(pageSize);
                        break;
                    case "dlzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_DLZS_Info(pageSize);
                        break;
                    case "dwzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_012DWZS_Info(pageSize);
                        break;
                    case "elzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_2LZS_Info(pageSize);
                        break;
                    case "ghzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_GHZS_Info(pageSize);
                        break;
                    case "hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_HZZS_Info(pageSize);
                        break;
                    case "kdzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_KDZS_Info(pageSize);
                        break;
                    case "lzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_012LZZS_Info(pageSize);
                        break;
                    case "q1jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_Q1JBZS_Info(pageSize);
                        break;
                    case "q1xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_Q1XTZS_Info(pageSize);
                        break;
                    case "q2jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_Q2JBZS_Info(pageSize);
                        break;
                    case "q2xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_Q2XTZS_Info(pageSize);
                        break;
                    case "q3jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_Q3JBZS_Info(pageSize);
                        break;
                    case "q3xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_Q3XTZS_Info(pageSize);
                        break;
                    case "xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_GD11X5_XTZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("gd11x5/" + id);
        }

        /// <summary>
        /// 辽宁11X5
        /// </summary>
        public ActionResult LN11X5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_JBZS_Info(pageSize);
                        break;
                    case "chzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_CHZS_Info(pageSize);
                        break;
                    case "dlzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_DLZS_Info(pageSize);
                        break;
                    case "dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_DXZS_Info(pageSize);
                        break;
                    case "elzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_2LZS_Info(pageSize);
                        break;
                    case "ghzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_GHZS_Info(pageSize);
                        break;
                    case "hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_HZZS_Info(pageSize);
                        break;
                    case "jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_JOZS_Info(pageSize);
                        break;
                    case "q1zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_Q1ZS_Info(pageSize);
                        break;
                    case "q2zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_Q2ZS_Info(pageSize);
                        break;
                    case "q3zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_LN11X5_Q3ZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("ln11x5/" + id);
        }

        /// <summary>
        /// 山东群英会
        /// </summary>
        public ActionResult SDQYH(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "dx" : id;
                switch (id)
                {
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_RXDX_Info(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_RXJO_Info(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_RXZH_Info(pageSize);
                        break;
                    case "chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_Chu3_Info(pageSize);
                        break;
                    case "sx1":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_SX1_Info(pageSize);
                        break;
                    case "sx2":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_SX2_Info(pageSize);
                        break;
                    case "sx3":
                        ViewBag.DataList = WCFClients.ChartClient.QuerySDQYH_SX3_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("sdqyh/" + id);
        }

        /// <summary>
        /// 江西时时彩
        /// </summary>
        public ActionResult JXSSC(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "s3zxzs" : id;
                switch (id)
                {
                    case "s2hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_2X_HZZS_Info(pageSize);
                        break;
                    case "s2zuxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_2X_ZuXZS_Info(pageSize);
                        break;
                    case "s2zxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_2X_ZXZS_Info(pageSize);
                        break;
                    case "s3c3ys":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_C3YS_Info(pageSize);
                        break;
                    case "s3dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_DXZS_Info(pageSize);
                        break;
                    case "s3hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_HZZS_Info(pageSize);
                        break;
                    case "s3jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_JOZS_Info(pageSize);
                        break;
                    case "s3kd":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_KD_Info(pageSize);
                        break;
                    case "s3zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_ZHZS_Info(pageSize);
                        break;
                    case "s3zuxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_ZuXZS_Info(pageSize);
                        break;
                    case "s3zxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_3X_ZXZS_Info(pageSize);
                        break;
                    case "s5hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_5X_HZZS_Info(pageSize);
                        break;
                    case "s5jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_5X_JBZS_Info(pageSize);
                        break;
                    case "dxds":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_DXDS_Info(pageSize);
                        break;
                    case "s1zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_JXSSC_1X_ZS_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("jxssc/" + id);
        }

        /// <summary>
        /// 江苏快3
        /// </summary>
        public ActionResult JSKS(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "hz" : id;
                switch (id)
                {
                    case "hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJSK3_HZ_Info(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJSK3_JBZS_Info(pageSize);
                        break;
                    case "xt":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJSK3_XT_Info(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJSK3_ZH_Info(pageSize);
                        break;
                    case "zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJSK3_ZHZS_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("jsks/" + id);
        }

        public ActionResult SDKLPK3(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;

                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_JBZS_Info(pageSize);
                        break;
                    case "zhxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_ZHXZS_Info(pageSize);
                        break;
                    case "hszs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_HSZS_Info(pageSize);
                        break;
                    case "dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_DXZS_Info(pageSize);
                        break;
                    case "jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_JOZS_Info(pageSize);
                        break;
                    case "zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_ZHZS_Info(pageSize);
                        break;
                    case "c3yzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_C3YZS_Info(pageSize);
                        break;
                    case "hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_HZZS_Info(pageSize);
                        break;
                    case "lxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_SDKLPK3_LXZS_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("sdklpk3/" + id);
        }

        /// <summary>
        /// 吉林快3
        /// </summary>
        public ActionResult JLK3(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "hz" : id;
                switch (id)
                {
                    case "hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJLK3_HZ_Info(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJLK3_JBZS_Info(pageSize);
                        break;
                    case "xt":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJLK3_XT_Info(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJLK3_ZH_Info(pageSize);
                        break;
                    case "zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryJLK3_ZHZS_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("jlk3/" + id);
        }

        /// <summary>
        /// 湖北快3
        /// </summary>
        public ActionResult HBK3(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "hz" : id;
                switch (id)
                {
                    case "hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHBK3_HZ_Info(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHBK3_JBZS_Info(pageSize);
                        break;
                    case "xt":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHBK3_XT_Info(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHBK3_ZH_Info(pageSize);
                        break;
                    case "zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHBK3_ZHZS_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("hbk3/" + id);
        }

        /// <summary>
        /// 七乐彩
        /// </summary>
        public ActionResult QLC(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQLC_Chu3(pageSize);
                        break;
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQLC_DX(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQLC_JBZS(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQLC_JO(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQLC_ZH(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("qlc/" + id);
        }

        /// <summary>
        /// 七星彩
        /// </summary>
        public ActionResult QXC(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQXC_Chu3(pageSize);
                        break;
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQXC_DX(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQXC_JBZS(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQXC_JO(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryQXC_ZH(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("qxc/" + id);
        }

        /// <summary>
        /// 排列pageSize
        /// </summary>
        public ActionResult PL5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL5_JBZS(pageSize);
                        break;
                    case "chu3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL5_Chu3(pageSize);
                        break;
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL5_DX(pageSize);
                        break;
                    case "hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL5_HZ(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL5_JO(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryPL5_ZH(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("pl5/" + id);
        }

        /// <summary>
        /// 重庆快乐十分
        /// </summary>
        public ActionResult CQKLSF(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "elzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_2LZS_Info(pageSize);
                        break;
                    case "slzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_3LZS_Info(pageSize);
                        break;
                    case "dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_DXZS_Info(pageSize);
                        break;
                    case "ghzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_GHZS_Info(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_JBZS_Info(pageSize);
                        break;
                    case "jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_JOZS_Info(pageSize);
                        break;
                    case "q1zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_Q1ZS_Info(pageSize);
                        break;
                    case "q3zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_Q3ZS_Info(pageSize);
                        break;
                    case "qjzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_QJZS_Info(pageSize);
                        break;
                    case "twzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_CQKLSF_TWZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("cqklsf/" + id);
        }

        /// <summary>
        /// 湖南快乐十分
        /// </summary>
        public ActionResult HNKLSF(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "elzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_2LZS_Info(pageSize);
                        break;
                    case "slzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_3LZS_Info(pageSize);
                        break;
                    case "dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_DXZS_Info(pageSize);
                        break;
                    case "ghzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_GHZS_Info(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_JBZS_Info(pageSize);
                        break;
                    case "jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_JOZS_Info(pageSize);
                        break;
                    case "q1zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_Q1ZS_Info(pageSize);
                        break;
                    case "q3zs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_Q3ZS_Info(pageSize);
                        break;
                    case "qjzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_QJZS_Info(pageSize);
                        break;
                    case "twzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HNKLSF_TWZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("hnklsf/" + id);
        }

        /// <summary>
        /// 好彩1
        /// </summary>
        public ActionResult HC1(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HC1_JBZS_Info(pageSize);
                        break;
                    case "sxjjfwzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HC1_SXJJFWZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("hc1/" + id);
        }

        /// <summary>
        /// 11运夺金
        /// </summary>
        public ActionResult SD11X5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_JBZS_Info(pageSize);
                        break;
                    case "chzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_CHZS_Info(pageSize);
                        break;
                    case "dlzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_DLZS_Info(pageSize);
                        break;
                    case "dwzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_012DWZS_Info(pageSize);
                        break;
                    case "elzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_2LZS_Info(pageSize);
                        break;
                    case "ghzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_GHZS_Info(pageSize);
                        break;
                    case "hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_HZZS_Info(pageSize);
                        break;
                    case "kdzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_KDZS_Info(pageSize);
                        break;
                    case "lzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_012LZZS_Info(pageSize);
                        break;
                    case "q1jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_Q1JBZS_Info(pageSize);
                        break;
                    case "q1xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_Q1XTZS_Info(pageSize);
                        break;
                    case "q2jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_Q2JBZS_Info(pageSize);
                        break;
                    case "q2xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_Q2XTZS_Info(pageSize);
                        break;
                    case "q3jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_Q3JBZS_Info(pageSize);
                        break;
                    case "q3xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_Q3XTZS_Info(pageSize);
                        break;
                    case "xtzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_YDJ11_XTZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("sd11x5/" + id);
        }

        /// <summary>
        /// 东方6+1
        /// </summary>
        public ActionResult DF6J1(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_DF6_1_JBZS_Info(pageSize);
                        break;
                    case "hzzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_DF6_1_HZZS_Info(pageSize);
                        break;
                    case "dxzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_DF6_1_DXZS_Info(pageSize);
                        break;
                    case "jozs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_DF6_1_JOZS_Info(pageSize);
                        break;
                    case "kdzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_DF6_1_KDZS_Info(pageSize);
                        break;
                    case "zhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_DF6_1_ZHZS_Info(pageSize);
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("df6j1/" + id);
        }

        /// <summary>
        /// 广东快乐十分
        /// </summary>
        public ActionResult GDKLSF(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_JBZS(pageSize);
                        break;
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_DX(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_JO(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_ZH(pageSize);
                        break;
                    case "dw1":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_DW1(pageSize);
                        break;
                    case "dw2":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_DW2(pageSize);
                        break;
                    case "dw3":
                        ViewBag.DataList = WCFClients.ChartClient.QueryGDKLSF_DW3(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("gdklsf/" + id);
        }

        /// <summary>
        /// 华东15选5
        /// </summary>
        public ActionResult HD15X5(string id)
        {
            try
            {
                var pageSize = string.IsNullOrEmpty(Request.QueryString["pageSize"]) ? this.PageSize : int.Parse(Request.QueryString["pageSize"]);
                id = string.IsNullOrEmpty(id) ? "jbzs" : id;
                switch (id)
                {
                    case "dx":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHD15X5_DX(pageSize);
                        break;
                    case "jo":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHD15X5_JO(pageSize);
                        break;
                    case "zh":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHD15X5_ZH(pageSize);
                        break;
                    case "hz":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHD15X5_HZ(pageSize);
                        break;
                    case "jbzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryHD15X5_JBZS(pageSize);
                        break;
                    case "chzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HD15X5_CH_Info(pageSize);
                        break;
                    case "lhzs":
                        ViewBag.DataList = WCFClients.ChartClient.QueryCache_HD15X5_LH_Info(pageSize);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                ViewBag.DataList = null;
            }
            return View("hd15x5/" + id);
        }


        /// <summary>
        /// 顶部彩种走势链接
        /// </summary>
        public PartialViewResult ThendLink(string id)
        {
            var phaseOrder = string.IsNullOrEmpty(Request["phaseOrder"]) ? "down" : Request["phaseOrder"];
            ViewBag.PhasseOrder = phaseOrder;
            var gameCode = Request.Url.Segments[2].Replace("/", "");
            var list = GetThendLinkCollection(gameCode.ToUpper());
            if (gameCode.ToUpper() == "CQSSC" && phaseOrder == "up")
            {
                foreach (var item in list.LinkList)
                {
                    var pagename = item.LinkUrl.Substring(item.LinkUrl.LastIndexOf('/') + 1);
                    var arr = pagename.Split('.');
                    item.LinkUrl = item.LinkUrl.Replace(pagename, arr[0] + "_up.html");
                }
            }
            ViewBag.thendLink = list;
            ViewBag.GameType = id;
            return PartialView();
        }

        public PartialViewResult ThendLink_2(string id)
        {
            ViewBag.PhasseOrder = "";
            //id: category_gamecode
            var p = id.Split('_');
            if (p.Length == 2)
            {
                ViewBag.GameCode = p[1];
                ViewBag.GameType = p[0];
                ViewBag.thendLink = GetThendLinkCollection(p[1].ToUpper());

            }
            else if (p.Length == 3)
            {
                ViewBag.GameCode = p[1];
                ViewBag.GameType = p[0];
                ViewBag.thendLink = GetThendLinkCollection(p[1].ToUpper());
                if (p[1].ToUpper() == "CQSSC" && p[2] == "up")
                {
                    ViewBag.PhasseOrder = "_up";
                }
            }
            //else if (p.Length == 3)
            //{
            //    ViewBag.GameCode = p[1];
            //    ViewBag.GameType = p[0];
            //    var list = GetThendLinkCollection(p[1].ToUpper());
            //    if (p[1].ToUpper() == "CQSSC" && p[2] == "up")
            //    {
            //        ViewBag.PhasseOrder = "_up";
            //        foreach (var item in list.LinkList)
            //        {
            //            var pagename = item.LinkUrl.Substring(item.LinkUrl.LastIndexOf('/') + 1);
            //            var arr = pagename.Split('.');
            //            item.LinkUrl = item.LinkUrl.Replace(pagename, arr[0] + "_up.html");
            //        }
            //    }
            //    ViewBag.thendLink = list;
            //}
            return PartialView();
        }

        public PartialViewResult ThendLink_3(string id)
        {
            //id: category _gamecode
            var p = id.Split('-');
            if (p.Length == 2)
            {
                ViewBag.thendLink = GetThendLinkCollection(p[1].ToUpper());
                ViewBag.GameCode = p[1];
                ViewBag.GameType = p[0];
            }
            return PartialView();
        }
    }
}
