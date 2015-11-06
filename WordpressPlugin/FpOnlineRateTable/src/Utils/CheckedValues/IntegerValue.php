<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'CheckedValue.php';


class IntegerValue extends CheckedValue {
    
    protected function sanitize( $value ) {
        
        if( is_numeric( $value ) ) {
            return (int) $value;
        }
        
        return null;
    }
    
    protected function validate( $value ) {
        return is_integer( $value );
    }
}