<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

require_once 'WidgetSettings.php';
require_once dirname(dirname(__DIR__)) . '/Utils/Polyfill/http_build_url.php';
require_once dirname(dirname(__DIR__)) . '/utils/CheckedValues/Culture.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\Culture;
use Respect\Validation\Rules\PostalCode;


/**
 * Description of AppSettings
 *
 * @author scharfenberg
 */
class AppSettings {
    
    const CULTURE = 'culture';
    const ISO3_COUNTRY_CODE = 'countryCode';
    const CARRIER_ID = 'carrierId';
    const ZIP_REGEX = 'zipRegex';
    const RATE_CALCULATION_START_URL = 'rateCalculationStartUrl';
    const RATE_CALCULATION_CALCULATE_URL = 'rateCalculationCalculateUrl';
    const RATE_CALCULATION_BACK_URL = 'rateCalculationBackUrl';
    const RATE_CALCULATION_UPDATE_WEIGHT_URL = 'rateCalculationUpdateWeightUrl';
    
    private $settings;
    
    
    public function __construct(WidgetSettings $widgetSettings) {
        
        $culture = $widgetSettings->getCurrentCulture();
        
        $this->settings = [
            self::CULTURE => $culture->getAsString(),
            self::ISO3_COUNTRY_CODE => $widgetSettings->getCurrentRateTableCountryCode(),
            self::CARRIER_ID => $widgetSettings->getCurrentRateTableCarrierId(),
            self::ZIP_REGEX => $this->zipRegexFromCulture($culture),
            self::RATE_CALCULATION_START_URL => $this->buildServiceUrl(
                    $widgetSettings, WidgetSettings::RATE_CALCULATION_START_PATH),
            self::RATE_CALCULATION_CALCULATE_URL => $this->buildServiceUrl(
                    $widgetSettings, WidgetSettings::RATE_CALCULATION_CALCULATE_PATH),
            self::RATE_CALCULATION_BACK_URL => $this->buildServiceUrl(
                    $widgetSettings, WidgetSettings::RATE_CALCULATION_BACK_PATH),
            self::RATE_CALCULATION_UPDATE_WEIGHT_URL => $this->buildServiceUrl(
                    $widgetSettings, WidgetSettings::RATE_CALCULATION_UPDATE_WEIGHT_PATH)
        ];
    }

    public function toArray() {
        return $this->settings;
    }
    
    
    private function buildServiceUrl(WidgetSettings $widgetSettings, $id) {
        
        $serviceUrl = $widgetSettings->getServiceUrl($id);
        $proxyUrl = plugins_url('3rdParty/resource-proxy/proxy.php');
        $joinedUrl = http_build_url('',
                [   'host' => $proxyUrl,
                    'query' => $serviceUrl],
                HTTP_URL_JOIN_QUERY);
        
        return $joinedUrl;
    }
    
    private function zipRegexFromCulture(Culture $culture) {
        
        $regexZip = null;
        if($culture->isValid()) {
            $country = $culture->getCountry();
            $countryCode = $country->getIso3166a2();
            $validator = new PostalCode($countryCode);
            $regexZip = $validator->regex;
        }
        
        return $regexZip;
    }
}
