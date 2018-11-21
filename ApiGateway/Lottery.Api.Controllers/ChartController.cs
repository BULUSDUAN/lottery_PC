using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kason.Sg.Core.ProxyGenerator;
using KaSon.FrameWork.Common;
using Lottery.Api.Controllers.CommonFilterActtribute;
using Lottery.ApiGateway.Model.HelpModel;
using Lottery.Base.Controllers;
using Microsoft.AspNetCore.Mvc;
using KaSon.FrameWork.ORM.Helper.WinNumber.ModelCollection;
using EntityModel.Enum;

namespace Lottery.Api.Controllers
{
    [Area("api")]
    [ReusltFilter]
    public class ChartController: BaseController
    {
        #region 双色球
        public async Task<IActionResult> SSQ([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse(){Code = ResponseCode.成功};
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_JiBenZouSi_InfoCollection>>(param, "api/Data/QueryCache_SSQ_JiBenZouSi_Info");
                        break;
                    case "daxiao":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_DX_InfoCollection>>(param, "api/Data/QueryCache_SSQ_DX_Info");
                        break;
                    case "chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_C3_InfoCollection>>(param, "api/Data/QueryCache_SSQ_C3_Info");
                        break;
                    case "hezhi":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_HeZhi_InfoCollection>>(param, "api/Data/QueryCache_SSQ_HeZhi_Info");
                        break;
                    case "jiou":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_JiOu_InfoCollection>>(param, "api/Data/QueryCache_SSQ_JiOu_Info");
                        break;
                    case "kuadu_12":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_KuaDu_1_6_InfoCollection>>(param, "api/Data/QueryCache_SSQ_KuaDu_1_6_Info");
                        break;
                    case "kuadu_23":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_KuaDu_1_6_InfoCollection>>(param, "api/Data/QueryCache_SSQ_KuaDu_1_6_Info");
                        break;
                    case "kuadu_34":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_KuaDu_1_6_InfoCollection>>(param, "api/Data/QueryCache_SSQ_KuaDu_1_6_Info");
                        break;
                    case "kuadu_45":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_KuaDu_1_6_InfoCollection>>(param, "api/Data/QueryCache_SSQ_KuaDu_1_6_Info");
                        break;
                    case "kuadu_56":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_KuaDu_1_6_InfoCollection>>(param, "api/Data/QueryCache_SSQ_KuaDu_1_6_Info");
                        break;
                    case "kuadu_sw":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_KuaDu_SW_InfoCollection>>(param, "api/Data/QueryCache_SSQ_KuaDu_SW_Info");
                        break;
                    case "zhihe":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SSQ_ZhiHe_InfoCollection>>(param, "api/Data/QueryCache_SSQ_ZhiHe_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResponseCode.失败;
                result.Value = null;
                result.Message = ex.Message;
                return Json(result);
            }
            return Json(result);
        }
        #endregion
        #region 福彩3D
        /// <summary>
        /// 福彩3D
        /// </summary>
        public async Task<IActionResult> FC3D([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize",string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "zxzs" : (string)p.id;
                switch (id)
                {
                    case "zxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_ZhiXuanZouSi_InfoCollection>>(param, "api/Data/QueryCache_FC3D_ZhiXuanZouSi_Info");
                        break;
                    case "zuxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_ZuXuanZouSi_InfoCollection>>(param, "api/Data/QueryCache_FC3D_ZuXuanZouSi_Info");
                        break;
                    case "dxxt":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_DXXT_InfoCollection>>(param, "api/Data/QueryCache_FC3D_DXXT_Info");
                        break;
                    case "dxhm":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_DXHM_InfoCollection>>(param, "api/Data/QueryCache_FC3D_DXHM_Info");
                        break;
                    case "joxt":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_JOXT_InfoCollection>>(param, "api/Data/QueryCache_FC3D_JOXT_Info");
                        break;
                    case "johm":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_JOHM_InfoCollection>>(param, "api/Data/QueryCache_FC3D_JOHM_Info");
                        break;
                    case "zhxt":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_ZHXT_InfoCollection>>(param, "api/Data/QueryCache_FC3D_ZHXT_Info");
                        break;
                    case "zhhm":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_ZHHM_InfoCollection>>(param, "api/Data/QueryCache_FC3D_ZHHM_Info");
                        break;
                    case "chu33":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_Chu32_InfoCollection>>(param, "api/Data/QueryCache_FC3D_Chu32_Info");
                        break;
                    case "chu32":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_Chu31_InfoCollection>>(param, "api/Data/QueryCache_FC3D_Chu31_Info");
                        break;
                    case "chu31":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_Chu33_InfoCollection>>(param, "api/Data/QueryCache_FC3D_Chu33_Info");
                        break;
                    case "hzfb":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_HZFB_InfoCollection>>(param, "api/Data/QueryCache_FC3D_HZFB_Info");
                        break;
                    case "hztz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_HZTZ_InfoCollection>>(param, "api/Data/QueryCache_FC3D_HZTZ_Info");
                        break;
                    case "hwzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_HZZS_InfoCollection>>(param, "api/Data/QueryCache_FC3D_HZZS_Info");
                        break;
                    case "kuadu13":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_KuaDu_13_InfoCollection>>(param, "api/Data/QueryCache_FC3D_KuaDu_13_Info");
                        break;
                    case "kuadu12":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_KuaDu_12_InfoCollection>>(param, "api/Data/QueryCache_FC3D_KuaDu_12_Info");
                        break;
                    case "kuadu23":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_KuaDu_23_InfoCollection>>(param, "api/Data/QueryCache_FC3D_KuaDu_23_Info");
                        break;
                    case "kuadu":
                        result.Value = await _serviceProxyProvider.Invoke<Task<FC3D_KuaDu_Z_InfoCollection>>(param, "api/Data/QueryCache_FC3D_KuaDu_Z_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResponseCode.失败;
                result.Value = null;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        #endregion
        #region 七乐彩
        /// <summary>
        /// 七乐彩
        /// </summary>
        public async Task<IActionResult> QLC([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                var pageSize = string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize);
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QLC_Chu3_InfoCollection>>(param, "api/Data/QueryQLC_Chu3");
                        break;
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QLC_DX_InfoCollection>>(param,"api/Data/QueryQLC_DX");
                        break;
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QLC_JBZS_InfoCollection>>(param, "api/Data/QueryQLC_JBZS");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QLC_JO_InfoCollection>>(param, "api/Data/QueryQLC_JO");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QLC_ZH_InfoCollection>>(param, "api/Data/QueryQLC_ZH");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResponseCode.失败;
                result.Value = null;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        #endregion
        #region 吉林快3
        /// <summary>
        /// 吉林快3
        /// </summary>
        public async Task<IActionResult> JLK3([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "hz" : (string)p.id;
                switch (id)
                {
                    case "hz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JLK3_HZ_InfoCollection>>(param, "api/Data/QueryJLK3_HZ_Info");
                        break;
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JLK3_JBZS_InfoCollection>>(param, "api/Data/QueryJLK3_JBZS_Info");
                        break;
                    case "xt":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JLK3_XT_InfoCollection>>(param, "api/Data/QueryJLK3_XT_Info");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JLK3_ZH_InfoCollection>>(param, "api/Data/QueryJLK3_ZH_Info");
                        break;
                    case "zhzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JLK3_ZHZS_InfoCollection>>(param, "api/Data/QueryJLK3_ZHZS_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResponseCode.失败;
                result.Value = null;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        #endregion
        #region 江苏快3
        /// <summary>
        /// 江苏快3
        /// </summary>
        public async Task<IActionResult> JSKS([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "hz" : (string)p.id;
                switch (id)
                {
                    case "hz":
                        result.Value =await _serviceProxyProvider.Invoke<Task<JLK3_ZHZS_InfoCollection>>(param, "api/Data/QueryJSK3_HZ_Info");
                        break;
                    case "jbzs":
                        result.Value =await _serviceProxyProvider.Invoke<Task<JSK3_JBZS_InfoCollection>>(param, "api/Data/QueryJSK3_JBZS_Info");
                        break;
                    case "xt":
                        result.Value =await _serviceProxyProvider.Invoke<Task<JSK3_XT_InfoCollection>>(param, "api/Data/QueryJSK3_XT_Info");
                        break;
                    case "zh":
                        result.Value =await _serviceProxyProvider.Invoke<Task<JSK3_ZH_InfoCollection>>(param, "api/Data/QueryJSK3_ZH_Info");
                        break;
                    case "zhzs":
                        result.Value =await _serviceProxyProvider.Invoke<Task<JSK3_ZHZS_InfoCollection>>(param, "api/Data/QueryJSK3_ZHZS_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 湖北快3
        /// <summary>
        /// 湖北快3
        /// </summary>
        public async Task<IActionResult> HBK3([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "hz" : (string)p.id;
                switch (id)
                {
                    case "hz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HBK3_HZ_InfoCollection>>(param, "api/Data/QueryHBK3_HZ_Info");
                        break;
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HBK3_JBZS_InfoCollection>>(param, "api/Data/QueryHBK3_JBZS_Info");
                        break;
                    case "xt":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HBK3_XT_InfoCollection>>(param, "api/Data/QueryHBK3_XT_Info");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HBK3_ZH_InfoCollection>>(param, "api/Data/QueryHBK3_ZH_Info");
                        break;
                    case "zhzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HBK3_ZHZS_InfoCollection>>(param, "api/Data/QueryHBK3_ZHZS_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResponseCode.失败;
                result.Value = null;
                result.Message = ex.Message;
            }
            return Json(result);
        }
        #endregion
        #region 群英会
        /// <summary>
        /// 山东群英会
        /// </summary>
        public async Task<IActionResult> SDQYH([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "dx" : (string)p.id;
                switch (id)
                {
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_RXDX_InfoCollection>>(param, "api/Data/QuerySDQYH_RXDX_Info");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_RXJO_InfoCollection>>(param, "api/Data/QuerySDQYH_RXJO_Info");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_RXZH_InfoCollection>>(param, "api/Data/QuerySDQYH_RXZH_Info");
                        break;
                    case "chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_Chu3_InfoCollection>>(param, "api/Data/QuerySDQYH_Chu3_Info");
                        break;
                    case "sx1":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_SX1_InfoCollection>>(param, "api/Data/QuerySDQYH_SX1_Info");
                        break;
                    case "sx2":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_SX2_InfoCollection>>(param, "api/Data/QuerySDQYH_SX2_Info");
                        break;
                    case "sx3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<SDQYH_SX3_InfoCollection>>(param, "api/Data/QuerySDQYH_SX3_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 好 彩 1
        /// <summary>
        /// 好彩1
        /// </summary>
        public async Task<IActionResult> HC1([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HC1_JBZS_InfoCollection>>(param, "api/Data/QueryCache_HC1_JBZS_Info");
                        break;
                    case "sxjjfwzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HC1_SXJJFWZS_InfoCollection>>(param, "api/Data/QueryCache_HC1_SXJJFWZS_Info");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 15选5
        /// <summary>
        /// 华东15选5
        /// </summary>
        public async Task<IActionResult> HD15X5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_DX_InfoCollection>>(param, "api/Data/QueryHD15X5_DX");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_JO_InfoCollection>>(param, "api/Data/QueryHD15X5_JO");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_ZH_InfoCollection>>(param, "api/Data/QueryHD15X5_ZH");
                        break;
                    case "hz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_HZ_InfoCollection>>(param, "api/Data/QueryHD15X5_HZ");
                        break;
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_JBZS_InfoCollection>>(param, "api/Data/QueryHD15X5_JBZS");
                        break;
                    case "chzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_CH_InfoCollection>>(param, "api/Data/QueryCache_HD15X5_CH_Info");
                        break;
                    case "lhzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<HD15X5_LH_InfoCollection>>(param, "api/Data/QueryCache_HD15X5_LH_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 东方6+1
        /// <summary>
        /// 东方6+1
        /// </summary>
        public async Task<IActionResult> DF6J1([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DF6_1_JBZS_InfoCollection>>(param, "api/Data/QueryCache_DF6_1_JBZS_Info");
                        break;
                    case "hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DF6_1_HZZS_InfoCollection>>(param, "api/Data/QueryCache_DF6_1_HZZS_Info");
                        break;
                    case "dxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DF6_1_DXZS_InfoCollection>>(param, "api/Data/QueryCache_DF6_1_DXZS_Info");
                        break;
                    case "jozs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DF6_1_JOZS_InfoCollection>>(param, "api/Data/QueryCache_DF6_1_JOZS_Info");
                        break;
                    case "kdzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DF6_1_KDZS_InfoCollection>>(param, "api/Data/QueryCache_DF6_1_KDZS_Info");
                        break;
                    case "zhzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DF6_1_ZHZS_InfoCollection>>(param, "api/Data/QueryCache_DF6_1_ZHZS_Info");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 大乐透
        /// <summary>
        /// 大乐透走势
        /// </summary>
        public async Task<IActionResult> DLT([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_JiBenZouSi_InfoCollection>>(param, "api/Data/QueryDLT_JiBenZouSi_Info");
                        break;
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_DX_InfoCollection>>(param, "api/Data/QueryDLT_DX_Info");
                        break;
                    case "jiou":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_JiOu_InfoCollection>>(param, "api/Data/QueryDLT_JiOu_Info");
                        break;
                    case "zhihe":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_ZhiHe_InfoCollection>>(param, "api/Data/QueryDLT_ZhiHe_Info");
                        break;
                    case "hezhi":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_HeZhi_InfoCollection>>(param, "api/Data/QueryDLT_HeZhi_Info");
                        break;
                    case "chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_Chu3_InfoCollection>>(param, "api/Data/QueryDLT_Chu3_Info");
                        break;
                    case "kuadu_sw":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_KuaDu_SW_InfoCollection>>(param, "api/Data/QueryDLT_KuaDu_SW_Info");
                        break;
                    case "kuadu_12":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_KuaDu_12_InfoCollection>>(param, "api/Data/QueryDLT_KuaDu_12_Info");
                        break;
                    case "kuadu_23":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_KuaDu_23_InfoCollection>>(param, "api/Data/QueryDLT_KuaDu_23_Info");
                        break;
                    case "kuadu_34":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_KuaDu_34_InfoCollection>>(param, "api/Data/QueryDLT_KuaDu_34_Info");
                        break;
                    case "kuadu_45":
                        result.Value = await _serviceProxyProvider.Invoke<Task<DLT_KuaDu_45_InfoCollection>>(param, "api/Data/QueryDLT_KuaDu_45_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 排列三
        /// <summary>
        /// 排列3
        /// </summary>
        public  async Task<IActionResult> PL3([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_JiBenZouSi_InfoCollection>>(param, "api/Data/QueryPL3_JiBenZouSi_Info");
                        break;
                    case "zxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_ZuXuanZouSi_InfoCollection>>(param, "api/Data/QueryPL3_ZuXuanZouSi_info");
                        break;
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_DX_InfoCollection>>(param, "api/Data/QueryPL3_DX_info");
                        break;
                    case "dxhm":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_DXHM_InfoCollection>>(param, "api/Data/QueryPL3_DXHM_info");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_JIOU_InfoCollection>>(param, "api/Data/QueryPL3_JIOU_info");
                        break;
                    case "johm":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_JOHM_InfoCollection>>(param, "api/Data/QueryPL3_JOHM_info");
                        break;
                    case "zhihe":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_ZhiHe_InfoCollection>>(param, "api/Data/QueryPL3_ZhiHe_info");
                        break;
                    case "zhhm":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_ZHHM_InfoCollection>>(param, "api/Data/QueryPL3_ZHHM_info");
                        break;
                    case "hezhi":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_HeiZhi_InfoCollection>>(param, "api/Data/QueryPL3_HeiZhi_Info");
                        break;
                    case "hztz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_HZTZ_InfoCollection>>(param, "api/Data/QueryPL3_PL3_HZTZ_Info");
                        break;
                    case "hzhw":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_HZHW_InfoCollection>>(param, "api/Data/QueryPL3_PL3_HZHW_Info");
                        break;
                    case "kdbs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_KuaDu_12_InfoCollection>>(param, "api/Data/QueryPL3_KuaDu_12_Info");
                        break;
                    case "kdsg":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_KuaDu_23_InfoCollection>>(param, "api/Data/QueryPL3_KuaDu_23_Info");
                        break;
                    case "kdbg":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_KuaDu_13_InfoCollection>>(param, "api/Data/QueryPL3_KuaDu_13_Info");
                        break;
                    case "chu33":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_Chu32_InfoCollection>>(param, "api/Data/QueryPL3_PL3_Chu32_Info");
                        break;
                    case "chu32":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_Chu31_InfoCollection>>(param, "api/Data/QueryPL3_PL3_Chu31_Info");
                        break;
                    case "chu31":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL3_Chu33_InfoCollection>>(param, "api/Data/QueryPL3_PL3_Chu33_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 排列五
        /// <summary>
        /// 排列pageSize
        /// </summary>
        public async Task<IActionResult> PL5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL5_JBZS_InfoCollection>>(param, "api/Data/QueryPL5_JBZS");
                        break;
                    case "chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL5_Chu3_InfoCollection>>(param, "api/Data/QueryPL5_Chu3");
                        break;
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL5_DX_InfoCollection>>(param, "api/Data/QueryPL5_DX");
                        break;
                    case "hz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL5_HZ_InfoCollection>>(param, "api/Data/QueryPL5_HZ");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL5_JO_InfoCollection>>(param, "api/Data/QueryPL5_JO");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<PL5_ZH_InfoCollection>>(param, "api/Data/QueryPL5_ZH");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 七星彩
        /// <summary>
        /// 七星彩
        /// </summary>
        public async Task<IActionResult> QXC([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QXC_Chu3_InfoCollection>>(param, "api/Data/QueryQXC_Chu3");
                        break;
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QXC_DX_InfoCollection>>(param, "api/Data/QueryQXC_DX");
                        break;
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QXC_JBZS_InfoCollection>>(param, "api/Data/QueryQXC_JBZS");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QXC_JO_InfoCollection>>(param, "api/Data/QueryQXC_JO");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<QXC_ZH_InfoCollection>>(param, "api/Data/QueryQXC_ZH");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 山东11选5
        /// <summary>
        /// 11运夺金
        /// </summary>
        public async Task<IActionResult> SD11X5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_JBZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_JBZS_Info");
                        break;
                    case "chzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_CHZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_CHZS_Info");
                        break;
                    case "dlzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_DLZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_DLZS_Info");
                        break;
                    case "dwzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_012DWZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_012DWZS_Info");
                        break;
                    case "elzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_2LZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_2LZS_Info");
                        break;
                    case "ghzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_GHZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_GHZS_Info");
                        break;
                    case "hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_HZZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_HZZS_Info");
                        break;
                    case "kdzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_KDZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_KDZS_Info");
                        break;
                    case "lzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_012LZZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_012LZZS_Info");
                        break;
                    case "q1jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_Q1JBZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_Q1JBZS_Info");
                        break;
                    case "q1xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_Q1XTZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_Q1XTZS_Info");
                        break;
                    case "q2jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_Q2JBZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_Q2JBZS_Info");
                        break;
                    case "q2xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_Q2XTZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_Q2XTZS_Info");
                        break;
                    case "q3jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_Q3JBZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_Q3JBZS_Info");
                        break;
                    case "q3xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_Q3XTZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_Q3XTZS_Info");
                        break;
                    case "xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<YDJ11_XTZS_InfoCollection>>(param, "api/Data/QueryCache_YDJ11_XTZS_Info");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 广东11选5
        /// <summary>
        /// 广东11X5
        /// </summary>
        public async Task<IActionResult> GD11X5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_JBZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_JBZS_Info");
                        break;
                    case "chzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_CHZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_CHZS_Info");
                        break;
                    case "dlzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_DLZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_DLZS_Info");
                        break;
                    case "dwzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_012DWZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_012DWZS_Info");
                        break;
                    case "elzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_2LZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_2LZS_Info");
                        break;
                    case "ghzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_GHZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_GHZS_Info");
                        break;
                    case "hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_HZZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_HZZS_Info");
                        break;
                    case "kdzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_KDZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_KDZS_Info");
                        break;
                    case "lzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_012LZZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_012LZZS_Info");
                        break;
                    case "q1jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_Q1JBZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_Q1JBZS_Info");
                        break;
                    case "q1xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_Q1XTZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_Q1XTZS_Info");
                        break;
                    case "q2jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_Q2JBZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_Q2JBZS_Info");
                        break;
                    case "q2xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_Q2XTZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_Q2XTZS_Info");
                        break;
                    case "q3jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_Q3JBZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_Q3JBZS_Info");
                        break;
                    case "q3xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_Q3XTZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_Q3XTZS_Info");
                        break;
                    case "xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GD11X5_XTZS_InfoCollection>>(param, "api/Data/QueryCache_GD11X5_XTZS_Info");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 江西11选5
        /// <summary>
        /// 江西11选5
        /// </summary>
        public async Task<IActionResult> JX11X5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "rxjbzs" : (string)p.id;
                switch (id)
                {
                    case "rxjbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RXJBZS_InfoCollection>>(param, "api/Data/QueryJX11X5_RXJBZS_Info");
                        break;
                    case "rxdx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RXDX_InfoCollection>>(param, "api/Data/QueryJX11X5_RXDX_Info");
                        break;
                    case "rxjo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RXJO_InfoCollection>>(param, "api/Data/QueryJX11X5_RXJO_Info");
                        break;
                    case "rxzh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RXZH_InfoCollection>>(param, "api/Data/QueryJX11X5_RXZH_Info");
                        break;
                    case "rxhz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RXHZ_InfoCollection>>(param, "api/Data/QueryJX11X5_RXHZ_Info");
                        break;
                    case "rxchu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Chu3_InfoCollection>>(param, "api/Data/QueryJX11X5_Chu3_Info");
                        break;
                    case "rx1":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RX1_InfoCollection>>(param, "api/Data/QueryJX11X5_RX1_Info");
                        break;
                    case "rx2":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RX2_InfoCollection>>(param, "api/Data/QueryJX11X5_RX2_Info");
                        break;
                    case "rx3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RX3_InfoCollection>>(param, "api/Data/QueryJX11X5_RX3_Info");
                        break;
                    case "rx4":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RX4_InfoCollection>>(param, "api/Data/QueryJX11X5_RX4_Info");
                        break;
                    case "rx5":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_RX5_InfoCollection>>(param, "api/Data/QueryJX11X5_RX5_Info");
                        break;
                    case "q3zx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3ZS_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3ZS_Info");
                        break;
                    case "q3zux":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3ZUS_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3ZUS_Info");
                        break;
                    case "q3dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3DX_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3DX_Info");
                        break;
                    case "q3jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3JO_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3JO_Info");
                        break;
                    case "q3zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3ZH_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3ZH_Info");
                        break;
                    case "q3chu3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3Chu3_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3Chu3_Info");
                        break;
                    case "q3hz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q3HZ_InfoCollection>>(param, "api/Data/QueryJX11X5_Q3HZ_Info");
                        break;
                    case "q2zx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q2ZS_InfoCollection>>(param, "api/Data/QueryJX11X5_Q2ZS_Info");
                        break;
                    case "q2zux":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q2ZUS_InfoCollection>>(param, "api/Data/QueryJX11X5_Q2ZUS_Info");
                        break;
                    case "q2hz":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JX11X5_Q2HZ_InfoCollection>>(param, "api/Data/QueryJX11X5_Q2HZ_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 重庆11选5
        /// <summary>
        /// 重庆11X5
        /// </summary>
        public async Task<IActionResult> CQ11X5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_JBZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_JBZS_Info");
                        break;
                    case "chzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_CHZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_CHZS_Info");
                        break;
                    case "dlzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_DLZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_DLZS_Info");
                        break;
                    case "dwzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_012DWZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_012DWZS_Info");
                        break;
                    case "elzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_2LZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_2LZS_Info");
                        break;
                    case "ghzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_GHZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_GHZS_Info");
                        break;
                    case "hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_HZZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_HZZS_Info");
                        break;
                    case "kdzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_KDZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_KDZS_Info");
                        break;
                    case "lzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_012LZZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_012LZZS_Info");
                        break;
                    case "q1jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_Q1JBZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_Q1JBZS_Info");
                        break;
                    case "q1xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_Q1XTZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_Q1XTZS_Info");
                        break;
                    case "q2jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_Q2JBZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_Q2JBZS_Info");
                        break;
                    case "q2xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_Q2XTZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_Q2XTZS_Info");
                        break;
                    case "q3jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_Q3JBZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_Q3JBZS_Info");
                        break;
                    case "q3xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_Q3XTZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_Q3XTZS_Info");
                        break;
                    case "xtzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQ11X5_XTZS_InfoCollection>>(param, "api/Data/QueryCache_CQ11X5_XTZS_Info");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 辽宁11选5
        /// <summary>
        /// 辽宁11X5
        /// </summary>
        public async Task<IActionResult> LN11X5([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_JBZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_JBZS_Info");
                        break;
                    case "chzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_CHZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_CHZS_Info");
                        break;
                    case "dlzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_DLZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_DLZS_Info");
                        break;
                    case "dxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_DXZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_DXZS_Info");
                        break;
                    case "elzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_2LZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_2LZS_Info");
                        break;
                    case "ghzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_GHZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_GHZS_Info");
                        break;
                    case "hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_HZZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_HZZS_Info");
                        break;
                    case "jozs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_JOZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_JOZS_Info");
                        break;
                    case "q1zs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_Q1ZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_Q1ZS_Info");
                        break;
                    case "q2zs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_Q2ZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_Q2ZS_Info");
                        break;
                    case "q3zs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<LN11X5_Q3ZS_InfoCollection>>(param, "api/Data/QueryCache_LN11X5_Q3ZS_Info");
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 重庆时时彩
        /// <summary>
        /// 重庆时时彩
        /// </summary>
        public async Task<IActionResult> CQSSC([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "s3zxzs" : (string)p.id;
                switch (id)
                {
                    case "s2hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_2X_HZZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_2X_HZZS_Info");
                        break;
                    case "s2zuxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_2X_ZuXZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_2X_ZuXZS_Info");
                        break;
                    case "s2zxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_2X_ZXZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_2X_ZXZS_Info");
                        break;
                    case "s3c3ys":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_C3YS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_C3YS_Info");
                        break;
                    case "s3dxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_DXZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_DXZS_Info");
                        break;
                    case "s3hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_HZZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_HZZS_Info");
                        break;
                    case "s3jozs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_JOZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_JOZS_Info");
                        break;
                    case "s3kd":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_KD_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_KD_Info");
                        break;
                    case "s3zhzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_ZHZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_ZHZS_Info");
                        break;
                    case "s3zuxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_ZuXZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_ZuXZS_Info");
                        break;
                    case "s3zxzs":
                        //result.Value = await _serviceProxyProvider.Invoke<Task<>>QueryCache_CQSSC_3X_ZXZS_Info_New(param,"api/Data/", phaseOrder);
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_3X_ZXZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_3X_ZXZS_Info");
                        break;
                    case "s5hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_5X_HZZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_5X_HZZS_Info");
                        break;
                    case "s5jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_5X_JBZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_5X_JBZS_Info");
                        break;
                    case "dxds":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_DXDS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_DXDS_Info");
                        break;
                    case "s1zs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<CQSSC_1X_ZS_InfoCollection>>(param, "api/Data/QueryCache_CQSSC_1X_ZS_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 江西时时彩
        /// <summary>
        /// 江西时时彩
        /// </summary>
        public async Task<IActionResult> JXSSC([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "s3zxzs" : (string)p.id;
                switch (id)
                {
                    case "s2hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_2X_HZZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_2X_HZZS_Info");
                        break;
                    case "s2zuxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_2X_ZuXZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_2X_ZuXZS_Info");
                        break;
                    case "s2zxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_2X_ZXZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_2X_ZXZS_Info");
                        break;
                    case "s3c3ys":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_C3YS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_C3YS_Info");
                        break;
                    case "s3dxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_DXZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_DXZS_Info");
                        break;
                    case "s3hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_HZZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_HZZS_Info");
                        break;
                    case "s3jozs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_JOZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_JOZS_Info");
                        break;
                    case "s3kd":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_KD_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_KD_Info");
                        break;
                    case "s3zhzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_ZHZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_ZHZS_Info");
                        break;
                    case "s3zuxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_ZuXZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_ZuXZS_Info");
                        break;
                    case "s3zxzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_3X_ZXZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_3X_ZXZS_Info");
                        break;
                    case "s5hzzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_5X_HZZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_5X_HZZS_Info");
                        break;
                    case "s5jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_5X_JBZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_5X_JBZS_Info");
                        break;
                    case "dxds":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_DXDS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_DXDS_Info");
                        break;
                    case "s1zs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<JXSSC_1X_ZS_InfoCollection>>(param, "api/Data/QueryCache_JXSSC_1X_ZS_Info");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        #region 广东快乐十分
        /// <summary>
        /// 广东快乐十分
        /// </summary>
        public async Task<IActionResult> GDKLSF([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        {
            var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
            try
            {
                var param = new Dictionary<string, object>();
                var p = JsonHelper.Decode(entity.Param);
                param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
                var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
                switch (id)
                {
                    case "jbzs":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_JBZS_InfoCollection>>(param, "api/Data/QueryGDKLSF_JBZS");
                        break;
                    case "dx":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_DX_InfoCollection>>(param, "api/Data/QueryGDKLSF_DX");
                        break;
                    case "jo":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_JO_InfoCollection>>(param, "api/Data/QueryGDKLSF_JO");
                        break;
                    case "zh":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_ZH_InfoCollection>>(param, "api/Data/QueryGDKLSF_ZH");
                        break;
                    case "dw1":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_DW1_InfoCollection>>(param, "api/Data/QueryGDKLSF_DW1");
                        break;
                    case "dw2":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_DW2_InfoCollection>>(param, "api/Data/QueryGDKLSF_DW2");
                        break;
                    case "dw3":
                        result.Value = await _serviceProxyProvider.Invoke<Task<GDKLSF_DW3_InfoCollection>>(param, "api/Data/QueryGDKLSF_DW3");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Value = null;
                result.Code = ResponseCode.失败;
            }
            return Json(result);
        }
        #endregion
        //#region 湖南快乐十分
        ///// <summary>
        ///// 湖南快乐十分
        ///// </summary>
        //public async Task<IActionResult> HNKLSF([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        //{
        //    var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
        //    try
        //    {
        //        var param = new Dictionary<string, object>();
        //        var p = JsonHelper.Decode(entity.Param);
        //        param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
        //        var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
        //        switch (id)
        //        {
        //            case "elzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_2LZS_Info");
        //                break;
        //            case "slzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_3LZS_Info");
        //                break;
        //            case "dxzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_DXZS_Info");
        //                break;
        //            case "ghzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_GHZS_Info");
        //                break;
        //            case "jbzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_JBZS_Info");
        //                break;
        //            case "jozs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_JOZS_Info");
        //                break;
        //            case "q1zs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_Q1ZS_Info");
        //                break;
        //            case "q3zs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_Q3ZS_Info");
        //                break;
        //            case "qjzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_QJZS_Info");
        //                break;
        //            case "twzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_HNKLSF_TWZS_Info");
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.Value = null;
        //        result.Code = ResponseCode.失败;
        //    }
        //    return Json(result);
        //}
        //#endregion
        //#region 重庆快乐十分
        ///// <summary>
        ///// 重庆快乐十分
        ///// </summary>
        //public async Task<IActionResult> CQKLSF([FromServices]IServiceProxyProvider _serviceProxyProvider, LotteryServiceRequest entity)
        //{
        //    var result = new LotteryServiceResponse() { Code = ResponseCode.成功 };
        //    try
        //    {
        //        var param = new Dictionary<string, object>();
        //        var p = JsonHelper.Decode(entity.Param);
        //        param.Add("pageSize", string.IsNullOrEmpty((string)p.pageSize) ? 0 : int.Parse((string)p.pageSize));
        //        var id = string.IsNullOrEmpty((string)p.id) ? "jbzs" : (string)p.id;
        //        switch (id)
        //        {
        //            case "elzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_2LZS_Info");
        //                break;
        //            case "slzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_3LZS_Info");
        //                break;
        //            case "dxzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_DXZS_Info");
        //                break;
        //            case "ghzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_GHZS_Info");
        //                break;
        //            case "jbzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_JBZS_Info");
        //                break;
        //            case "jozs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_JOZS_Info");
        //                break;
        //            case "q1zs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_Q1ZS_Info");
        //                break;
        //            case "q3zs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_Q3ZS_Info");
        //                break;
        //            case "qjzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_QJZS_Info");
        //                break;
        //            case "twzs":
        //                result.Value = await _serviceProxyProvider.Invoke<Task<>>(param, "api/Data/QueryCache_CQKLSF_TWZS_Info");
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.Message;
        //        result.Value = null;
        //        result.Code = ResponseCode.失败;
        //    }
        //    return Json(result);
        //}
        //#endregion

    }
}
