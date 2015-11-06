<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin;

require_once 'Widget.php';
require_once dirname(__DIR__) . '/Utils/CheckedValues/UrlValue.php';
require_once dirname(__DIR__) . '/Utils/GlobalLogger.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\CheckedValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue\UrlValue;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;


/**
 * Description of WidgetSettings
 *
 * @author scharfenberg
 */
class WidgetSettings {
    
    private $settings;
    private $widget;
    private $config;
    
    
    public function __construct(Widget $widget, array $instance) {
        
        $this->widget = $widget;
        $this->config = $widget->config();
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
    
    public function toJson() {
        
        $array = [];
        array_walk($this->settings,
                function (CheckedValue $value, $key) use (&$array) {
                    $array[$key] = $value->getIfValid();
                });
        
        $result = json_encode($array);
        
        return $result;
    }
    
    private function &serviceUrl_() {
        return $this->settings[$this->config->serviceUrlId()];
    }
    public function serviceUrl() {
        return $this->serviceUrl_()->get();
    }
    public function seviceUrlIfValid() {
        return $this->serviceUrl_()->getIfValid();
    }
    public function serviceUrlHtmlId() {
        return $this->widget->get_field_id($this->config->serviceUrlId());
    }
    public function serviceUrlHtmlName() {
        return $this->widget->get_field_name($this->config->serviceUrlId());
    }
   
    
    private function initSettings() {
        
        $this->settings = [];
        $this->settings[$this->config->serviceUrlId()] = new UrlValue();
    }
    
    private function set($key, $value) {
        
        switch($key) {
            case $this->config->serviceUrlId():
                $serviceUrl = &$this->serviceUrl_();
                $serviceUrl = new UrlValue($value);
                break;
            
            default:
                GlobalLogger::addWarning(
                        "Cannot set FpOnlineRateTable Widget settings value",
                        ["key" => $key, "value" => $value]);
        }
    }
}
