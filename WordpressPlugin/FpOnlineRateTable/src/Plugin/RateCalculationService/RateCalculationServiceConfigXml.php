<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService;

require_once 'IRateCalculationServiceConfig.php';


/**
 * Description of RateCalculationServiceConfigXml
 *
 * @author scharfenberg
 */
class RateCalculationServiceConfigXml implements IRateCalculationServiceConfig {

    private $defaultGetActiveRateTablesPath;
    private $defaultRateCalculationStartPath;
    private $defaultRateCalculationCalculatePath;
    private $defaultRateCalculationBackPath;
    private $defaultRateCalculationUpdateWeightPath;
    
    
    public function __construct(\SimpleXMLElement $config) {
        
        $this->defaultGetActiveRateTablesPath
                = (string)$config->RateCalculationServiceService->DefaultGetActiveRateTablesPath;
        $this->defaultRateCalculationStartPath
                = (string)$config->RateCalculationServiceService->DefaultRateCalculationStartPath;
        $this->defaultRateCalculationCalculatePath
                = (string)$config->RateCalculationServiceService->DefaultRateCalculationCalculatePath;
        $this->defaultRateCalculationBackPath
                = (string)$config->RateCalculationServiceService->DefaultRateCalculationBackPath;
        $this->defaultRateCalculationUpdateWeightPath
                = (string)$config->RateCalculationServiceService->DefaultRateCalculationUpdateWeightPath;
    }
    
    /**
     * The standard path (without hostname) of the GetActiveRateTables resource
     */
    public function defaultGetActiveRateTablesPath() {
        return $this->defaultGetActiveRateTablesPath;
    }
    
    /**
     * The standard path (without hostname) of the RateCalculationStart resource
     */
    public function defaultRateCalculationStartPath() {
        return $this->defaultRateCalculationStartPath;
    }
    
    /**
     * The standard path (without hostname) of the RateCalculationCalculate
     * resource
     */
    public function defaultRateCalculationCalculatePath() {
        return $this->defaultRateCalculationCalculatePath;
    }
    
    /**
     * The standard path (without hostname) of the RateCalculationBack resource
     */
    public function defaultRateCalculationBackPath() {
        return $this->defaultRateCalculationBackPath;
    }
    
    /**
     * The standard path (without hostname) of the RateCalculationUpdateWeight
     * resource
     */
    public function defaultRateCalculationUpdateWeightPath() {
        return $this->defaultRateCalculationUpdateWeightPath;
    }
}
