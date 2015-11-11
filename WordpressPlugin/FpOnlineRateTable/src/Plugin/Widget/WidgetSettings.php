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
require_once dirname(dirname(__DIR__)) . '/Utils/GlobalLogger.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\CheckedValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;


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
    const BASE_SERVICE_URL = 'service-url';
    const GET_ACTIVE_RATETABLES_PATH = 'get-active-rate-tables-path';
    const RATE_CALCULATION_START_PATH = 'rate-calculation-start-path';
    const RATE_CALCULATION_CALCULATE_PATH = 'rate-calculation-calculate-path';
    const RATE_CALCULATION_BACK_PATH = 'rate-calculation-back-path';
    const RATE_CALCULATION_UPDATE_WEIGHT_PATH = 'rate-calculation-update-weight-path';
    
    private static $idToCheckedValueClass = [
        self::TITLE => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\TextValue',
        self::BASE_SERVICE_URL => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue',
        self::GET_ACTIVE_RATETABLES_PATH => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue',
        self::RATE_CALCULATION_START_PATH => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue',
        self::RATE_CALCULATION_CALCULATE_PATH => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue',
        self::RATE_CALCULATION_BACK_PATH => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue',
        self::RATE_CALCULATION_UPDATE_WEIGHT_PATH => '\FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue'
    ];
    
    private $settings;
    private $widget;
    private $serviceConfig;
    
    
    public function __construct(Widget $widget, array $instance) {
        
        $this->widget = $widget;
        $this->serviceConfig = $widget->rateCalculatorService()->config();
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
    
    public function validToArray() {
        
        $result = [];
        array_walk($this->settings,
                function (CheckedValue $value, $key) use (&$result) {
                    $result[$key] = $value->getIfValid();
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
    
    public function htmlId($id) {
        return $this->widget->get_field_id($id);
    }
    
    public function htmlName($id) {
        return $this->widget->get_field_name($id);
    }
   
    
    private function initSettings() {
        
        $this->settings = [];
        $this->set(self::TITLE, '');
        $this->set(self::BASE_SERVICE_URL, '');
        $this->set(self::GET_ACTIVE_RATETABLES_PATH,
                $this->serviceConfig->defaultGetActiveRateTablesPath());
        $this->set(self::RATE_CALCULATION_START_PATH,
                $this->serviceConfig->defaultRateCalculationStartPath());
        $this->set(self::RATE_CALCULATION_CALCULATE_PATH,
                $this->serviceConfig->defaultRateCalculationCalculatePath());
        $this->set(self::RATE_CALCULATION_BACK_PATH,
                $this->serviceConfig->defaultRateCalculationBackPath());
        $this->set(self::RATE_CALCULATION_UPDATE_WEIGHT_PATH,
                $this->serviceConfig->defaultRateCalculationUpdateWeightPath());
    }
}
