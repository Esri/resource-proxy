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
    public function resourceUrl() {
        return $this->widgetSettings->getIfValid(
                WidgetSettings::RESOURCE_URL);
    }
}
