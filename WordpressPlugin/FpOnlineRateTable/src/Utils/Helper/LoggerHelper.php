<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper;

/**
 * Description of loggerHelper
 *
 * @author scharfenberg
 */
trait LoggerHelper {
    
    private $loggingEnabled;
    private $canWriteLog;
    
    
    public function loggingEnabled() {
        return $this->loggingEnabled;
    }
    
    protected function setLoggingEnabled($value) {
        $this->loggingEnabled = !!$value;
    }
    
    public function canWriteLog() {
        return $this->canWriteLog;
    }
    
    protected function setCanWriteLog($value) {
        $this->canWriteLog = !!$value;
    }
    
    protected function extractMessage($thing) {
        
        if($thing instanceof \Exception) {
            return $thing->getMessage();
        }
        
        return $thing;
    }
    
    private function levelNameToValueMapping() {
        
        static $map = null;
        if(!isset($map)) {
            $refl = new \ReflectionClass(
                    'FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILogger');
            $map = $refl->getConstants();
        }
        
        return $map;
    }
    
    private function levelValueToNameMapping() {
        
        static $map = null;
        if(!isset($map)) {
            $map = array_flip($this->levelNameToValueMapping());
        }
        
        return $map;
    }
    
    protected function levelName($level) {
        
        $map = $this->levelValueToNameMapping();
        if(array_key_exists($level, $map)) {
            return $map[$level];
        }
    }
}
