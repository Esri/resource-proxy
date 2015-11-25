<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils;

/**
 *
 * @author scharfenberg
 */
interface IGlobalLoggerConfig {
    
    function logFile();
    
    function logLevel();
    
    function loggerName();
    
    function maxFiles();
}
