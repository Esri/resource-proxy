<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

require_once 'WidgetSettings.php';
require_once dirname(dirname(__DIR__)) . '/Utils/Polyfill/http_build_url.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/Culture.php';
require_once dirname(dirname(__DIR__)) . '/Utils/Helper/UrlHelper.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\Culture;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\UrlHelper;
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
    const RESOURCE_URL = 'rateCalculationUrl';
    const MAX_WEIGHT = 'maxWeight';
    
    private $settings;
    
    
    public function __construct(WidgetSettings $widgetSettings) {
        
        $culture = $widgetSettings->getCurrentCulture();
        
        $this->settings = [
            self::CULTURE => $culture->getAsString(),
            self::ISO3_COUNTRY_CODE => $widgetSettings->getCurrentRateTableCountryCode(),
            self::CARRIER_ID => $widgetSettings->getCurrentRateTableCarrierId(),
            self::ZIP_REGEX => $this->zipRegexFromCulture($culture),
            self::RESOURCE_URL => $this->buildServiceUrl($widgetSettings),
            self::MAX_WEIGHT => $widgetSettings->getIfValid(
                    WidgetSettings::MAX_WEIGHT)
        ];
    }

    public function toArray() {
        return $this->settings;
    }
    
    
    private function buildServiceUrl(WidgetSettings $widgetSettings) {
        
        $proxyPath = $widgetSettings->getWidgetConfig()->serviceProxyPath();
        $proxyUrl = plugins_url($proxyPath, dirname(dirname(__DIR__)));
        $serviceUrl = $widgetSettings->getIfValid(WidgetSettings::RESOURCE_URL);
        
        $result = UrlHelper::buildRestMethodUrl(
                $proxyUrl, null, $serviceUrl);
        
        return $result;
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
