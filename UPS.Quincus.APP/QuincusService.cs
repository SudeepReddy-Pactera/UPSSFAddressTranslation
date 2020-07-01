using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UPS.DataObjects.Common;
using UPS.DataObjects.Shipment;
using UPS.Quincus.APP.Configuration;
using UPS.Quincus.APP.ProxyConnections;
using UPS.Quincus.APP.Request;
using UPS.Quincus.APP.Response;

namespace UPS.Quincus.APP
{
    public class QuincusService
    {

        public static QuincusTokenDataResponse GetToken(QuincusParams quincusParams)
        {
            QuincusTokenDataResponse quincusTokenDataResponse = QuincusProxy.GetToken(quincusParams);
            return quincusTokenDataResponse;

        }

        public static QuincusTranslatedAddressResponse GetTranslationAddress(IQuincusAddressTranslationRequest quincusAddressTranslationRequest, QuincusParams quincusParams)
        {

            QuincusTranslatedAddressResponse quincusTranslatedAddressResponse = QuincusProxy.GetTranslatedAddressResponse(quincusAddressTranslationRequest, quincusParams);

            return quincusTranslatedAddressResponse;
        }

        public static QuincusResponse GetGeoCodeReponseFromQuincus(QuincusGeoCodeDataRequest quincusGeoCodeDataRequest)
        {
            QuincusResponse quincusReponse = QuincusProxy.GetQuincusResponse(quincusGeoCodeDataRequest);
            return quincusReponse;
        }

        public static GetSFCreateOrderServiceResponse SFExpressCreateOrder(SFCreateOrderServiceRequest sFCreateOrderServiceRequest)
        {
            GetSFCreateOrderServiceResponse getSFCreateOrderServiceResponse =  new SFExpressProxy().getSFCreateOrderServiceResponse(sFCreateOrderServiceRequest).Result;

            return getSFCreateOrderServiceResponse;
        }



        public static SFTranslationAPIResponse GetSFTranslatedAddress(SFTranslationParams sfTranslationParams)
        {
            SFTranslationAPIResponse sfTranslatedAddress = QuincusProxy.GetSFTranslatedAddress(sfTranslationParams);
            return sfTranslatedAddress;

        }



        public static GetSFCancelOrderServiceResponse SFExpressCancelOrder(SFCancelOrderServiceRequest sFCancelOrderServiceRequest)
        {
            GetSFCancelOrderServiceResponse getSFCancelOrderServiceResponse = new SFExpressProxy().getSFCancelOrderServiceResponse(sFCancelOrderServiceRequest).Result;

            return getSFCancelOrderServiceResponse;
        }

        public async Task<SFTranslationAPIResponse> GETSFTranslatedAddresses(SFTranslationParams sfTranslationParams)
        {
            
            SFTranslationAPIResponse response = await new SFExpressProxy().GetSFTranslatedAddress(sfTranslationParams);

            return response;
        }

    }
}
