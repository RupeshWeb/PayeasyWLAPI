using AttributeRouting.Web.Http;
using BusinessEntities;
using BusinessServices;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebPlatApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TransservicesController : ApiController
    {
        #region Private variable.
        private readonly IBBPSServices _bbpsServices;
        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public TransservicesController(IBBPSServices bbpsServices)
        {
            _bbpsServices = bbpsServices;
        }

        #endregion

        #region Private Method
        [POST("BillerCategories")]
        public HttpResponseMessage BillerCategories()
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                return SubBillerCategories();
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [POST("CategoriesBiller")]
        public HttpResponseMessage CategoriesBiller(short categoryCode)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (categoryCode > 0)
                {
                    return SubCategoryBiller(categoryCode);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Category code is required.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [POST("BillerParameters")]
        public HttpResponseMessage BillerParameters(short billerCode)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (billerCode > 0)
                {
                    return SubBillerParams(billerCode);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Biller code is required.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        [POST("BillerParameterGrouping")]
        public HttpResponseMessage BillerParameterGrouping(int billerCode, int billerParam)
        {
            APIReponseEntity _response = new APIReponseEntity();
            try
            {
                if (billerCode > 0 && billerParam > 0)
                {
                    return SubBillerParamGrouping(billerCode, billerParam);
                }
                else
                {
                    _response.StatusCode = clsVariables.APIStatus.Failed;
                    _response.Message = "Biller code is required.";
                }
            }
            catch (Exception ex)
            {
                _response.StatusCode = clsVariables.APIStatus.Exception;
                _response.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, _response);
        }

        #endregion

        #region Private Method
        private HttpResponseMessage SubBillerCategories()
        {
            var result = _bbpsServices.BillerCategory();
            var response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        private HttpResponseMessage SubCategoryBiller(short categoryCode)
        {
            var result = _bbpsServices.BillerCategory(categoryCode);
            var response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        private HttpResponseMessage SubBillerParams(int billerCode)
        {
            var result = _bbpsServices.BillerParameters(billerCode);
            var response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }

        private HttpResponseMessage SubBillerParamGrouping(int billerCode, int paramsId)
        {
            var result = _bbpsServices.BillerParameterGrouping(billerCode, paramsId);
            var response = Request.CreateResponse(HttpStatusCode.OK, result);
            return response;
        }
        #endregion
    }
}
