---
title: Rest API documentation
has_children: true
---
*[generated documentation]*  
### BitfinexRestClient  
*Client for accessing the Bitfinex API.*
  
***
*General endpoints*  
**[IBitfinexRestClientGeneralApi](GeneralApi/IBitfinexRestClientGeneralApi.html) GeneralApi { get; }**  
***
*Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.*  
**void SetApiCredentials(ApiCredentials credentials);**  
***
*Spot endpoints*  
**[IBitfinexRestClientSpotApi](SpotApi/IBitfinexRestClientSpotApi.html) SpotApi { get; }**  
