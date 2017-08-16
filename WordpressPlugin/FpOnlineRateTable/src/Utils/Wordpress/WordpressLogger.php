<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once dirname(__DIR__) . '/Helper/ILogger.php';
require_once dirname(__DIR__) . '/Helper/ILoggerConfig.php';
require_once dirname(__DIR__) . '/Helper/LoggerHelper.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILogger;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\LoggerHelper;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILoggerConfig;


/**
 * Description of GlobalLogger
 *
 * @author scharfenberg
 */
class WordpressLogger implements ILogger {
    use LoggerHelper;
    
    const DEFAULT_LOGGER_NAME = __CLASS__;
    const DEFAULT_LOG_LEVEL = ILogger::DEBUG;
    
    
    private $logLevel;
    private $loggerName;
    
    
    public function __construct(
            $loggerName = self::DEFAULT_LOGGER_NAME,
            $logLevel = self::DEFAULT_LOG_LEVEL) {
        
        $this->logLevel = $logLevel;
        $this->loggerName = $loggerName;
        $this->setLoggingEnabled(defined('WP_DEBUG') && (WP_DEBUG === true));
        $this->setCanWriteLog(true);
    }
    
    static public function create(
            $loggerName = self::DEFAULT_LOGGER_NAME,
            $logLevel = self::DEFAULT_LOG_LEVEL) {
        return new self($loggerName, $logLevel);
    }
    
    static public function createFromConfig(ILoggerConfig $config) {
        return self::create($config->loggerName(), $config->loggerName());
    }
    
    public function getLogLevel() {
        return $this->logLevel;
    }
    
    public function getLoggerName() {
        return $this->loggerName;
    }
    
    /**
     * Adds a log record at the DEBUG level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addDebug($message) {
        $this->log($message, ILogger::DEBUG);
    }
    
    /**
     * Adds a log record at the INFO level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addInfo($message) {
        $this->log($message, ILogger::INFO);
    }

    /**
     * Adds a log record at the NOTICE level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addNotice($message) {
        $this->log($message, ILogger::NOTICE);
    }

    /**
     * Adds a log record at the WARNING level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addWarning($message) {
        $this->log($message, ILogger::WARNING);
    }

    /**
     * Adds a log record at the ERROR level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addError($message) {
        $this->log($message, ILogger::ERROR);
    }

    /**
     * Adds a log record at the CRITICAL level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addCritical($message) {
        $this->log($message, ILogger::CRITICAL);
    }

    /**
     * Adds a log record at the ALERT level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addAlert($message) {
        $this->log($message, ILogger::ALERT);
    }

    /**
     * Adds a log record at the EMERGENCY level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addEmergency($message) {
        $this->log($message, ILogger::EMERGENCY);
    }
    
    private function log($thing, $level) {
        
        if($this->canWriteLog() && ($level <= $this->logLevel)){
            $msg = $this->buildMessage($thing, $level);
            error_log($msg);
        }
    }
    
    private function buildMessage($thing, $level) {
        
        $levelName = $this->levelName($level);
        $timeStamp = (new DateTime())->format(DateTime::W3C);
        $msg = $this->extractMessage($thing);
        $fullMsg = "[{$timeStamp}] {$this->loggerName}.{$levelName}: {$msg}";
        
        return $fullMsg;
    }
}
