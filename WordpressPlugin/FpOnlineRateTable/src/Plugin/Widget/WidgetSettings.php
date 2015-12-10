<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

require_once 'Widget.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/UrlValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/TextValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/IntegerValue.php';
require_once dirname(dirname(__DIR__)) . '/Utils/CheckedValues/Culture.php';
require_once dirname(dirname(__DIR__)) . '/Utils/GlobalLogger.php';
require_once dirname(dirname(__DIR__)) . '/Utils/Polyfill/http_build_url.php';
require_once dirname(__DIR__) . '/RateCalculationService/RateCalculationService.php';
require_once dirname(__DIR__) . '/RateCalculationService/RateTableInfo.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\CheckedValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;
use FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService\RateCalculationService;
use FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService\RateTableInfo;
use FP\Web\Portal\FpOnlineRateTable\src\Plugin\RateCalculationService\ServiceException;


class UnkownWidgetSettingsIdException extends \RuntimeException {
    public function __construct($id, $previous = null) {
        parent::__construct(
                'Widget Settings id "' . $id .'" is unknown',
                0, $previous);
    }
}

/**
 * Description of WidgetSettings
 *
 * @author scharfenberg
 */
class WidgetSettings {
    
    const TITLE = 'title';
    const CURRENT_RATE_TABLE_CULTURE = 'current-rate-table-culture';
    const RESOURCE_URL = 'resource-url';
    const MAX_WEIGHT = 'max-weight';
    
    private static $idToCheckedValueClass = [
        self::TITLE => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\TextValue',
        self::CURRENT_RATE_TABLE_CULTURE => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\Culture',
        self::RESOURCE_URL => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue',
        self::MAX_WEIGHT => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\IntegerValue'
    ];
    
    private $settings;
    private $widget;
    private $widgetConfig;
    private $rateTables = null;
    private $rateTableServiceError = null;
    
    
    public function __construct(Widget $widget, array $instance) {
        
        $this->widget = $widget;
        $this->widgetConfig = $widget->config();
        $this->initSettings();
        $this->mergeArray($instance);
    }
    
    public function mergeArray(array $instance) {
        
        foreach(array_keys($this->settings) as $key) {
            if(array_key_exists($key, $instance)) {
                $this->set($key, $instance[$key]);
            }
        }
    }
    
    public function toArray() {
        
        $result = [];
        array_walk($this->settings,
                function (CheckedValue $value, $key) use (&$result) {
                    $result[$key] = $value->get();
                });
        
        return $result;
    }
    
    private static function idConstants() {
        
        static $constants = null;
        
        if(!is_array($constants)) {
            $refl = new \ReflectionClass(__CLASS__);
            $constants = $refl->getConstants();
        }
        
        return $constants;
    }
    
    private function validateId($id) {
        
        if(!in_array($id, self::idConstants())) {
            throw new UnkownWidgetSettingsIdException($id);
        }
    }
    
    private function internalAccess($id) {
        
        $this->validateId($id);
        
        if(!array_key_exists($id, $this->settings)) {
            $this->settings[$id] = null;
            GlobalLogger::addWarning(
                    'accessing previously unused widget settings id',
                    ['id' => $id]);
        }
        
        return $this->settings[$id];
    }
    
    private function set($id, $value) {
        
        $this->validateId($id);
        $class = self::$idToCheckedValueClass[$id];
        $this->settings[$id] = new $class($value);
    }
    
    public function get($id) {
        return $this->internalAccess($id)->get();
    }
    
    public function getIfValid($id) {
        return $this->internalAccess($id)->getIfValid();
    }
    
    public function isValid($id) {
        return $this->internalAccess($id)->isValid();
    }
    
    public function htmlId($id) {
        return $this->widget->get_field_id($id);
    }
    
    public function htmlName($id) {
        return $this->widget->get_field_name($id);
    }
    
    public function getRateTableChoices() {
        
        static $choices = null;
        if(!isset($choices)) {
            $choices = [];
            array_walk($this->getRateTables(),
                    function(RateTableInfo $rateTable) use (&$choices) {
                        $culture = $rateTable->Culture->getAsString();
                        $variant = $rateTable->Variant->getAsString();
                        $choices[$culture] = $variant;
                    });
        }
                
        return $choices;
    }
    
    /**
     * Returns the currently selected culture as Culture object
     * @return Culture
     */
    public function getCurrentCulture() {
        
        $culture = $this->internalAccess(self::CURRENT_RATE_TABLE_CULTURE);
        
        return $culture;
    }
    
    public function getCurrentRateTableCountryCode() {
        
        $currentRateTable = $this->getCurrentRateTableInfo();
        if(isset($currentRateTable)) {
            $culture = $currentRateTable->Culture;
            $country = $culture->getCountry();
            $countryCode = $country->getIso3166a3();
            return $countryCode;
        }
        
        return null;
    }
    
    public function getCurrentRateTableCarrierId() {
        
        $currentRateTable = $this->getCurrentRateTableInfo();
        if(isset($currentRateTable)) {
            $carrierId = $currentRateTable->CarrierId->get();
            return $carrierId;
        }
        
        return null;
    }
    
    
    public function hasServiceError() {
        
        // access rate table first so we have the chance to report an error
        $this->getRateTables();
        
        return ($this->rateTableServiceError instanceof \Exception);
    }
    
    public function getServiceError() {
        
        // access rate table first so we have the chance to report an error
        $this->getRateTables();
        
        return $this->rateTableServiceError->getMessage();
    }
    
    public function getWidgetConfig() {
        return $this->widgetConfig;
    }
   
    
    private function initSettings() {
        
        $this->settings = [];
        $this->set(self::TITLE, '');
        $this->set(self::CURRENT_RATE_TABLE_CULTURE, '');
        $this->set(self::RESOURCE_URL, '');
        $this->set(self::MAX_WEIGHT, 70000);
    }
    

    
    private function getRateTables() {
        
        if(!isset($this->rateTables)) {
            try {
                $rateCalculationServiceConfig
                        = new RateCalculationServiceConfigWidget($this);
                $rateService = new RateCalculationService(
                        $rateCalculationServiceConfig);
                $this->rateTables
                        = $rateService->GetActiveTables(new \DateTime());
            } catch (ServiceException $ex) {
                $this->rateTableServiceError = $ex;
            }
        }
        
        return $this->rateTables;
    }
    
    private function getCurrentRateTableInfo() {
        
        static $currentRateTableInfo = null;
        
        if(!isset($currentRateTableInfo)) {
            $culture = $this->getCurrentCulture();
            if($culture->isValid()) {
                $found = array_filter($this->getRateTables(),
                        function(RateTableInfo $rateTable) use ($culture) {
                            return ($rateTable->Culture->get() === $culture->get());
                        });
            
                // currently just return the first entry found. Should we throw an
                // error if none or more than one rate table was found?
                if(is_array($found) && count($found) > 0) {
                    $currentRateTableInfo = array_values($found)[0];
                }
            }
        }
        
        // return null if no current rate table could be found
        return $currentRateTableInfo;
    }
}
