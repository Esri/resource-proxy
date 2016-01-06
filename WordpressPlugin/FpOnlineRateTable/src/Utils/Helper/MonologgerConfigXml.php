<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils;

require_once 'LoggerConfigXml.php';
require_once 'IMonologgerConfig.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\IMonologgerConfig;

/**
 * Description of GlobalLoggerConfigXml
 *
 * @author scharfenberg
 */
class MonologgerConfigXml
        extends LoggerConfigXml
        implements IMonologgerConfig {
    
    private $logFile;
    private $maxFiles;
    
    
    public function __construct(\SimpleXMLElement $config) {
        
        parent::__construct($config);
        
        $this->logFile = (string)$config->GlobalLogger->LogFile;
        $this->maxFiles = (string)$config->GlobalLogger->MaxFiles;
    }
    
    function logFile() {
        return $this->logFile;
    }
    
    function maxFiles() {
        return $this->maxFiles;
    }
}