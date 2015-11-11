<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

/**
 *
 * @author scharfenberg
 */
interface IWidgetConfig {
    
    /**
     * This is the Wordpress internal Id of the widget
     */
    public function id();
    
    /**
     * This is the html class name used for the widget container
     */
    public function htmlClass();
    
    /**
     * This is the shortcode used to place the widget on a page
     */
    public function shortcode();
    
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
