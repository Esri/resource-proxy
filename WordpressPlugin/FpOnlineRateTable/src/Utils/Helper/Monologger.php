<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper;

require 'ILogger.php';
require 'IMonologgerConfig.php';
require 'LoggerHelper.php';
require dirname(dirname(dirname(__DIR__))) . '/vendor/autoload.php';

use Monolog\Logger;
use Monolog\Handler\RotatingFileHandler;


/**
 * Description of Monologger
 *
 * @author scharfenberg
 */
class Monologger implements ILogger {
    use LoggerHelper;
    
    static private $logLevelMap = [
        Logger::DEBUG => ILogger::DEBUG,
        Logger::INFO => ILogger::INFO,
        Logger::NOTICE => ILogger::NOTICE,
        Logger::WARNING => ILogger::WARNING,
        Logger::ERROR => ILogger::ERROR,
        Logger::CRITICAL => ILogger::CRITICAL,
        Logger::ALERT => ILogger::ALERT,
        Logger::EMERGENCY => ILogger::EMERGENCY
    ];
    
    const DEFAULT_LOGGER_NAME = __CLASS__;
    const DEFAULT_LOG_FILE = self::DEFAULT_LOGGER_NAME;
    const DEFAULT_LOG_LEVEL = Logger::DEBUG;
    const DEFAULT_MAX_FILES = 30;
    const DEFAULT_RELATIVE_PATH_BASE = __DIR__;
    
    
    private $logger;
    private $handler;
    private $context = [];
    
    
    public function __construct(
            $loggerName = self::DEFAULT_LOGGER_NAME,
            $logFile = self::DEFAULT_LOG_FILE,
            $maxFiles = self::DEFAULT_MAX_FILES,
            $logLevel = self::DEFAULT_LOG_LEVEL,
            $relativePathBase = self::DEFAULT_RELATIVE_PATH_BASE) {
        
        $this->logger = new Logger($loggerName);
        
        $path = $this->getLogFilePath($logFile, $relativePathBase);
        $this->setCanWriteLog($path);
        $this->setLoggingEnabled(true);
        if($this->canWriteLog()) {
            $this->handler = new RotatingFileHandler(
                    $path, $maxFiles, $logLevel);
            $this->logger->pushHandler($this->handler);
        }
    }
    
    static public function createFromConfig(IMonologgerConfig $config,
            $relativePathBase = self::DEFAULT_RELATIVE_PATH_BASE) {
        
        return new self(
                $config->loggerName(),
                $config->logFile(),
                $config->maxFiles(),
                $config->logLevel(),
                $relativePathBase);
    }
    
    public function getLogLevel() {
        return self::$logLevelMap[$this->handler->getLevel()];
    }
    
    public function getLoggerName() {
        return $this->logger->getName();
    }
    
    /**
     * Adds a log record at the DEBUG level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addDebug($message)
    {
        return $this->logger->addDebug(
                $this->extractMessage($message), $this->context);
    }
    
    /**
     * Adds a log record at the INFO level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addInfo($message)
    {
        return $this->logger->addInfo(
                $this->extractMessage($message), $this->context);
    }

    /**
     * Adds a log record at the NOTICE level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addNotice($message)
    {
        return $this->logger->addNotice(
                $this->extractMessage($message), $this->context);
    }

    /**
     * Adds a log record at the WARNING level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addWarning($message)
    {
        return $this->logger->addWarning(
                $this->extractMessage($message), $this->context);
    }

    /**
     * Adds a log record at the ERROR level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addError($message)
    {
        return $this->logger->addError(
                $this->extractMessage($message), $this->context);
    }

    /**
     * Adds a log record at the CRITICAL level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addCritical($message)
    {
        return $this->logger->addCritical(
                $this->extractMessage($message), $this->context);
    }

    /**
     * Adds a log record at the ALERT level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addAlert($message)
    {
        return $this->logger->addAlert(
                $this->extractMessage($message), $this->context);
    }

    /**
     * Adds a log record at the EMERGENCY level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addEmergency($message)
    {
        return $this->logger->addEmergency(
                $this->extractMessage($message), $this->context);
    }
    
    
    private function checkFileWritable($file) {
        
        // check if we can create a log file
        $modFile = $file . '_checkPermission';
        
        // we need to suppress the error output of touch. Otherwise we get
        // warnings in the log if the file cannot be touched.
        $writable = @touch($modFile);
        if($writable) {
            unlink($modFile);
        }
        
        return $writable;
    }
    
    private function getLogFilePath($logFile, $relativePathBase) {
        
        // First assume that the logfile was specified using an absolute path
        $writable = $this->checkFileWritable($logFile);
        if($writable) {
            return $logFile;
        }
        
        // Now assume that it is a path relative to the specified base dir
        $path = $relativePathBase . '/' . $logFile;
        $writable = $this->checkFileWritable($path);
        if($writable) {
            return $path;
        }
        
        // so we have no write access - either due to a erroneous path or due to
        // file permissions.
        return null;
    }
}
