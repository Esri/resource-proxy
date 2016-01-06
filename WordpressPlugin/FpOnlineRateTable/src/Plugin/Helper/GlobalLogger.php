<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Helper;

require_once dirname(dirname(__DIR__)) . '/Utils/Helper/ILogger.php';
require_once dirname(dirname(__DIR__)) . '/Utils/Helper/ILoggerConfig.php';
require_once dirname(dirname(__DIR__)) . '/Utils/Wordpress/WordpressLogger.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\WordpressLogger;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILoggerConfig;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\ILogger;



/**
 * Description of GlobalLogger
 * Currently we use a Wordpress logger as writing custom log files into a
 * Wordpress plugin directory is problematic and can be a security issue.
 * (http://ottopress.com/2011/tutorial-using-the-wp_filesystem/)
 *
 * @author scharfenberg
 */
abstract class GlobalLogger {
    
    const DEFAULT_LOGGER_NAME = __CLASS__;
    const DEFAULT_LOG_LEVEL = ILogger::DEBUG;
   
    
    static private $loggerName = self::DEFAULT_LOGGER_NAME;
    static private $logLevel = self::DEFAULT_LOG_LEVEL;
    
    
    static public function instance() {
    
        static $instance = null;
        if(!isset($instance)) {
            $instance = new WordpressLogger(
                    self::$loggerName, self::$logLevel);
        }
        
        return $instance;
    }
    
    static public function canWriteLog() {
        return self::instance()->canWriteLog();
    }
    
    /**
     * Adds a log record at the DEBUG level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addDebug($message) {
        return self::instance()->addDebug($message);
    }
    
    /**
     * Adds a log record at the INFO level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addInfo($message) {
        return self::instance()->addInfo($message);
    }

    /**
     * Adds a log record at the NOTICE level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addNotice($message) {
        return self::instance()->addNotice($message);
    }

    /**
     * Adds a log record at the WARNING level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addWarning($message) {
        return self::instance()->addWarning($message);
    }

    /**
     * Adds a log record at the ERROR level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addError($message) {
        return self::instance()->addError($message);
    }

    /**
     * Adds a log record at the CRITICAL level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addCritical($message) {
        return self::instance()->addCritical($message);
    }

    /**
     * Adds a log record at the ALERT level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addAlert($message) {
        return self::instance()->addAlert($message);
    }

    /**
     * Adds a log record at the EMERGENCY level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    static public function addEmergency($message) {
        return self::instance()->addEmergency($message);
    }
    
    
    static public function initilaLoggerName() {
        return self::$loggerName;
    }
    static public function setInitialLoggerName($value) {
        self::$loggerName = $value;
    }
    
    static public function initialLogLevel() {
        return self::$logLevel;
    }
    static public function setInitialLogLevel($value) {
        self::$logLevel = $value;
    }
    
    static public function readInitialConfig(ILoggerConfig $config) {
        
        self::setInitialLogLevel($config->logLevel());
        self::setInitialLoggerName($config->loggerName());
    }
}
