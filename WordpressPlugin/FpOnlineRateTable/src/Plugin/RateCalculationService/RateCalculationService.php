<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService;

require_once 'IRateCalculationServiceConfig.php';


/**
 * Description of RateCalculationService
 *
 * @author scharfenberg
 */
class RateCalculationService {
    
    private $config;
    
    
    public function __construct(IRateCalculationServiceConfig $config) {
        $this->config = $config;
    }
    
    public function config() {
        return $this->config;
    }
}
