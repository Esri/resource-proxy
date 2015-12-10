<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils;

require_once 'IGlobalLoggerConfig.php';


/**
 * Description of GlobalLoggerConfigXml
 *
 * @author scharfenberg
 */
class GlobalLoggerConfigXml implements IGlobalLoggerConfig {
    
    private $logFile;
    private $logLevel;
    private $loggerName;
    private $maxFiles;
    
    
    public function __construct(\SimpleXMLElement $config) {
        
        $this->logFile = (string)$config->GlobalLogger->LogFile;
        $this->logLevel = (string)$config->GlobalLogger->LogLevel;
        $this->loggerName = (string)$config->GlobalLogger->LoggerName;
        $this->maxFiles = (string)$config->GlobalLogger->MaxFiles;
    }
    
    function logFile() {
        return $this->logFile;
    }
    
    function logLevel() {
        return $this->logLevel;
    }
    
    function loggerName() {
        return $this->loggerName;
    }
    
    function maxFiles() {
        return $this->maxFiles;
    }
}