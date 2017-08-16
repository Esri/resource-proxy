<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper;


/**
 * Description of GlobalLogger
 * Inspired by Monolog
 *
 * @author scharfenberg
 */
interface ILogger {
     
    const DEBUG = 8;

    /**
     * Interesting events
     *
     * Examples: User logs in, SQL logs.
     */
    const INFO = 7;

    /**
     * Uncommon events
     */
    const NOTICE = 6;

    /**
     * Exceptional occurrences that are not errors
     *
     * Examples: Use of deprecated APIs, poor use of an API,
     * undesirable things that are not necessarily wrong.
     */
    const WARNING = 5;

    /**
     * Runtime errors
     */
    const ERROR = 4;

    /**
     * Critical conditions
     *
     * Example: Application component unavailable, unexpected exception.
     */
    const CRITICAL = 3;

    /**
     * Action must be taken immediately
     *
     * Example: Entire website down, database unavailable, etc.
     * This should trigger the SMS alerts and wake you up.
     */
    const ALERT = 2;

    /**
     * Urgent alert.
     */
    const EMERGENCY = 1;
    
    
    public function canWriteLog();
    
    public function getLogLevel();
    
    public function getLoggerName();
    
    /**
     * Adds a log record at the DEBUG level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addDebug($message);
    
    /**
     * Adds a log record at the INFO level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addInfo($message);

    /**
     * Adds a log record at the NOTICE level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addNotice($message);

    /**
     * Adds a log record at the WARNING level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addWarning($message);

    /**
     * Adds a log record at the ERROR level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addError($message);

    /**
     * Adds a log record at the CRITICAL level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addCritical($message);

    /**
     * Adds a log record at the ALERT level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addAlert($message);

    /**
     * Adds a log record at the EMERGENCY level.
     *
     * @param  string  $message The log message
     * @return Boolean Whether the record has been processed
     */
    public function addEmergency($message);
}
