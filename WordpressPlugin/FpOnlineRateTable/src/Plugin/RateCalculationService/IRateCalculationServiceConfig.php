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
     * The base service URL (i.e. protocol, hostname, port and api path but no
     * resource path)
     */
    public function baseUrl();
    
    /**
     * Resource path (relative to baseUrl) of the GetActiveRateTables resource
     */
    public function getActiveRateTablesPath();
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationStart resource
     */
    public function rateCalculationStartPath();
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationCalculate
     * resource
     */
    public function rateCalculationCalculatePath();
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationBack resource
     */
    public function rateCalculationBackPath();
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationUpdateWeight
     * resource
     */
    public function rateCalculationUpdateWeightPath();
}
