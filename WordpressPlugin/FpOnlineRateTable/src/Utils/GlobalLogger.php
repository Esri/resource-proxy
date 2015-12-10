<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils;

require dirname(dirname(__DIR__)) . '/vendor/autoload.php';
require 'IGlobalLoggerConfig.php';

use Monolog\Logger;
use Monolog\Handler\RotatingFileHandler;


/**
 * Description of GlobalLogger
 *
 * @author scharfenberg
 */
class GlobalLogger {
    
    const DEFAULT_LOGGER_NAME = __CLASS__;
    const DEFAULT_LOG_FILE = self::DEFAULT_LOGGER_NAME;
    const DEFAULT_LOG_LEVEL = Logger::DEBUG;
    const DEFAULT_MAX_FILES = 30;
    
    
    static private $instance;
    
    static private $loggerName = self::DEFAULT_LOGGER_NAME;
    static private $logFile = self::DEFAULT_LOG_FILE;
    static private $maxFiles = self::DEFAULT_MAX_FILES;
    static private $logLevel = self::DEFAULT_LOG_LEVEL;
    
    private $logger;
    private $canWriteLog;
    
    
    static public function initilaLoggerName() {
        return self::$loggerName;
    }
    static public function setInitialLoggerName($value) {
        self::$loggerName = $value;
    }
    
    static public function initialogFile() {
        return self::$logFile;
    }
    static public function setInitialLogFile($value) {
        self::$logFile = $value;
    }
    
    static public function initialLogLevel() {
        return self::$logLevel;
    }
    static public function setInitialLogLevel($value) {
        self::$logLevel = $value;
    }
    
    static public function initialMaxFiles() {
        return self::$maxFiles;
    }
    static public function setInitialMaxFiles($value) {
        self::$maxFiles = $value;
    }
    
    static public function readInitialConfig(IGlobalLoggerConfig $config) {
        
        self::setInitialLogFile($config->logFile());
        self::setInitialLogLevel($config->logLevel());
        self::setInitialLoggerName($config->loggerName());
        self::setInitialMaxFiles($config->maxFiles());
    }
    
    static public function canWriteLog() {
        return self::instance()->canWriteLog;
    }
    
    
    private function __construct($loggerName, $logFile, $maxFiles, $logLevel) {
        
        $this->logger = new Logger($loggerName);
        
        $this->canWriteLog = $this->checkLogFileWritable($logFile);
        if($this->canWriteLog) {
            $handler = new RotatingFileHandler($logFile, $maxFiles, $logLevel);
            $this->logger->pushHandler($handler);
        }
    }
    
    private function checkFileWritable($file) {
        
        // check if we can create a log file
        $modFile = $file . '_checkPermission';
        $writable = touch($modFile);
        if($writable) {
            unlink($modFile);
        }
        
        return $writable;
    }
    
    private function checkLogFileWritable($logFile) {
        
        // First treat the log file path as absolute path
        $writable = $this->checkFileWritable($logFile);
        if($writable) {
            return true;
        }
        
        // Now assume that it is a path relative to the plugin directory
        $pluginDir = dirname(dirname(dirname(plugin_dir_path(__FILE__))));
        $writable = $this->checkFileWritable($pluginDir . '/' . $logFile);
        
        return $writable;
    }
    
    static private function instance() {
    
        if(empty(self::$instance)) {
            self::$instance = new self(self::$loggerName,
                    self::$logFile, self::$maxFiles, self::$logLevel);
        }
        
        return self::$instance;
    }
    
    static private function extractMessage($thing) {
        
        if($thing instanceof \Exception) {
            return $thing->getMessage();
        }
        
        return $thing;
    }
    
    
    /**
     * Adds a log record at the DEBUG level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addDebug($message, array $context = array())
    {
        return self::instance()->logger->addDebug(
                self::extractMessage($message), $context);
    }
    
    /**
     * Adds a log record at the INFO level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addInfo($message, array $context = array())
    {
        return self::instance()->logger->addInfo(
                self::extractMessage($message), $context);
    }

    /**
     * Adds a log record at the NOTICE level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addNotice($message, array $context = array())
    {
        return self::instance()->logger->addNotice(
                self::extractMessage($message), $context);
    }

    /**
     * Adds a log record at the WARNING level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addWarning($message, array $context = array())
    {
        return self::instance()->logger->addWarning(
                self::extractMessage($message), $context);
    }

    /**
     * Adds a log record at the ERROR level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addError($message, array $context = array())
    {
        return self::instance()->logger->addError(
                self::extractMessage($message), $context);
    }

    /**
     * Adds a log record at the CRITICAL level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addCritical($message, array $context = array())
    {
        return self::instance()->logger->addCritical(
                self::extractMessage($message), $context);
    }

    /**
     * Adds a log record at the ALERT level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addAlert($message, array $context = array())
    {
        return self::instance()->logger->addAlert(
                self::extractMessage($message), $context);
    }

    /**
     * Adds a log record at the EMERGENCY level.
     *
     * @param  string  $message The log message
     * @param  array   $context The log context
     * @return Boolean Whether the record has been processed
     */
    static public function addEmergency($message, array $context = array())
    {
        return self::instance()->logger->addEmergency(
                self::extractMessage($message), $context);
    }
}
