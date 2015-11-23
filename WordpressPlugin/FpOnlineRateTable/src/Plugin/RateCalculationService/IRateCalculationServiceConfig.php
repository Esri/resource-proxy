<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService;

/**
 *
 * @author scharfenberg
 */
interface IRateCalculationServiceConfig {
    
    /**
     * The resource URL (i.e. protocol, hostname, port and resource path but 
     * without a method name)
     */
    public function resourceUrl();
}
