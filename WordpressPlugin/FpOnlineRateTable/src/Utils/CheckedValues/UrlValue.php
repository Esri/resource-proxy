<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'CheckedValue.php';


class UrlValue extends CheckedValue {
    
    protected function validate( $value ) {
      
        $sanitized = filter_var( $value, FILTER_VALIDATE_URL );
        
        return $value === $sanitized;
    }
}