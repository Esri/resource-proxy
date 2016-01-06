<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils;

require_once 'ILoggerConfig.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILoggerConfig;


/**
 * Description of GlobalLoggerConfigXml
 *
 * @author scharfenberg
 */
class LoggerConfigXml implements ILoggerConfig {
    
    private $logLevel;
    private $loggerName;
    
    
    public function __construct(\SimpleXMLElement $config) {
        
        $this->logLevel = (string)$config->GlobalLogger->LogLevel;
        $this->loggerName = (string)$config->GlobalLogger->LoggerName;
    }
    
    function logLevel() {
        return $this->logLevel;
    }
    
    function loggerName() {
        return $this->loggerName;
    }
}