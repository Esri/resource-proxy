<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper;


/**
 * Description of Callable
 *
 * @author scharfenberg
 */
abstract class CallableHelper {
    
    // If the specified argument is callable (i.e. a callback), call it without
    // any arguments and return the result. If it is not callable return the
    // argument itself.
    static public function callOrReturn($obj) {
        
        if(is_callable($obj)) {
            return call_user_func($obj);
        }
        
        return $obj;
    }
}
