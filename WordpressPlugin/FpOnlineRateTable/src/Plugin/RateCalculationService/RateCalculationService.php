<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService;

require_once 'IRateCalculationServiceConfig.php';
require_once 'RateTableInfo.php';
require_once 'RateTableInfo.php';


class ServiceException extends \RuntimeException {}
class GeneralServiceException extends ServiceException {}
class CouldNotConnectException extends ServiceException {}
class CouldNotDeserializeException extends ServiceException {}

class ResourceNotFoundException extends ServiceException {
    public function __construct($htmlCode, $previous = null) {
        parent::__construct(
                "Could not access resource, html response code is: {$htmlCode}",
                0, $previous);
    }
}


/**
 * Description of RateCalculationService
 *
 * @author scharfenberg
 */
class RateCalculationService {
    
    CONST GET_ACTIVE_TABLES_METHOD = 'GetActiveTables';
    
    private $config;
    
    
    public function __construct(IRateCalculationServiceConfig $config) {
        $this->config = $config;
    }
    
    public function config() {
        return $this->config;
    }
    
    public function GetActiveTables(\DateTime $clientDate) {
        
        $restClient = $this->getRestClient();
        $dateAsString = $clientDate->format('m/d/Y');
        $response = $restClient->get(self::GET_ACTIVE_TABLES_METHOD,
                ['clientUtcDate' => $dateAsString]);
        
        if(!($response instanceof \RestClient)) {
            throw new GeneralServiceException(
                    "Could not interpret answer of Rest Client");
        }
        
        if(false === $response->response) {
            throw new CouldNotConnectException($response->error);
        }
        
        if($response->info->http_code >= 400) {
            throw new ResourceNotFoundException($response->info->http_code);
        }
        
        $decoded = json_decode($response->response);
        if(null == $decoded) {
            throw new CouldNotDeserializeException(
                    "Error during decoding JSON string");
        }
        
        // if we got a singular result create an array first
        if(!is_array($decoded)) {
            $decoded = [$decoded];
        }
        
        // transfer the result array into an array of proper objects
        $result = array_map(
            [RateTableInfo::fqcn(), 'createFromStdClass'],
            $decoded);
        
        return $result;
    }
    
    private function getRestClient() {
        
        static $client = null;
        if(!isset($client)) {
            $client = new \RestClient( [
                'base_url' => $this->config->resourceUrl()
            ]);
        }
        
        return $client;
    }
}
