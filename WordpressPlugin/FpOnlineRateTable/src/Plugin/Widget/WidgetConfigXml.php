<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

require_once 'IWidgetConfig.php';


/**
 * Description of WidgetConfigXml
 *
 * @author scharfenberg
 */
class WidgetConfigXml implements IWidgetConfig {

    private $id;
    private $htmlClass;
    private $shortcode;
    private $defaultGetActiveRateTablesPath;
    private $defaultRateCalculationStartPath;
    private $defaultRateCalculationCalculatePath;
    private $defaultRateCalculationBackPath;
    private $defaultRateCalculationUpdateWeightPath;
    
    
    public function __construct(\SimpleXMLElement $config) {
        
        $this->id = (string)$config->FpOnlineRateCalculatorWidget->Id;
        $this->htmlClass
                = (string)$config->FpOnlineRateCalculatorWidget->HtmlClass;
        $this->shortcode
                = (string)$config->FpOnlineRateCalculatorWidget->Shortcode;
        $this->defaultGetActiveRateTablesPath
                = (string)$config->FpOnlineRateCalculatorWidget->DefaultGetActiveRateTablesPath;
        $this->defaultRateCalculationStartPath
                = (string)$config->FpOnlineRateCalculatorWidget->DefaultRateCalculationStartPath;
        $this->defaultRateCalculationCalculatePath
                = (string)$config->FpOnlineRateCalculatorWidget->DefaultRateCalculationCalculatePath;
        $this->defaultRateCalculationBackPath
                = (string)$config->FpOnlineRateCalculatorWidget->DefaultRateCalculationBackPath;
        $this->defaultRateCalculationUpdateWeightPath
                = (string)$config->FpOnlineRateCalculatorWidget->DefaultRateCalculationUpdateWeightPath;
    }
    
    /**
     * This is the Wordpress internal Id of the widget
     */
    public function id() {
        return $this->id;
    }
    
    /**
     * This is the html class name used for the widget container
     */
    public function htmlClass() {
        return $this->htmlClass;
    }
    
    /**
     * This is the shortcode used to place the widget on a page
     */
    public function shortcode() {
        return $this->shortcode;
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
