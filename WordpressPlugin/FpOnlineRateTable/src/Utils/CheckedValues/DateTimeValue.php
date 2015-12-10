<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'CheckedValue.php';


class DateTimeValue extends CheckedValue {
    
    private $format;
    
    
    public function __construct($format,
            $value = null, $id = null, $emptyIsValid = false) {
        
        $this->format = $format;
        parent::__construct($value, $id, $emptyIsValid);
    }
    
    protected function validate($value) {
        return ($value instanceof \DateTime);
    }
    
    protected function sanitize($value) {
        
        $result = \DateTime::createFromFormat($this->format, $value);
        
        return $result;
    }
}
