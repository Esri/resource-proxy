<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

require_once dirname(__DIR__) . '/RateCalculationService/IRateCalculationServiceConfig.php';
require_once 'WidgetSettings.php';

use FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService\IRateCalculationServiceConfig;


/**
 *
 * @author scharfenberg
 */
class RateCalculationServiceConfigWidget
    implements IRateCalculationServiceConfig {

    private $widgetSettings;

    
    public function __construct(WidgetSettings $settings) {
        $this->widgetSettings = $settings;
    }
    
    /**
     * The base service URL (i.e. protocol, hostname, port and api path but no
     * resource path)
     */
    public function baseUrl() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::BASE_SERVICE_URL);
    }
    
    /**
     * Resource path (relative to baseUrl) of the GetActiveRateTables resource
     */
    public function getActiveRateTablesPath() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::GET_ACTIVE_RATETABLES_PATH);
    }
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationStart resource
     */
    public function rateCalculationStartPath() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::RATE_CALCULATION_START_PATH);
    }
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationCalculate
     * resource
     */
    public function rateCalculationCalculatePath() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::RATE_CALCULATION_CALCULATE_PATH);
    }
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationBack resource
     */
    public function rateCalculationBackPath() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::RATE_CALCULATION_BACK_PATH);
    }
    
    /**
     * Resource path (relative to baseUrl) of the RateCalculationUpdateWeight
     * resource
     */
    public function rateCalculationUpdateWeightPath() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::RATE_CALCULATION_UPDATE_WEIGHT_PATH);
    }
}
