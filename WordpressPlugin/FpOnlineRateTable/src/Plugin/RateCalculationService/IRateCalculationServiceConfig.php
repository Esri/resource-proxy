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
     * The standard path (without hostname) of the GetActiveRateTables resource
     */
    public function defaultGetActiveRateTablesPath();
    
    /**
     * The standard path (without hostname) of the RateCalculationStart resource
     */
    public function defaultRateCalculationStartPath();
    
    /**
     * The standard path (without hostname) of the RateCalculationCalculate
     * resource
     */
    public function defaultRateCalculationCalculatePath();
    
    /**
     * The standard path (without hostname) of the RateCalculationBack resource
     */
    public function defaultRateCalculationBackPath();
    
    /**
     * The standard path (without hostname) of the RateCalculationUpdateWeight
     * resource
     */
    public function defaultRateCalculationUpdateWeightPath();
}
