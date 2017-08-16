<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper;

require_once 'ILoggerConfig.php';


/**
 *
 * @author scharfenberg
 */
interface IMonologgerConfig extends ILoggerConfig {
    
    function logFile();
    
    function maxFiles();
}
