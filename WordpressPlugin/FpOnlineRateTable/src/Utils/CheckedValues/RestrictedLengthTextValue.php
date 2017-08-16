<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'TextValue.php';


class RestrictedLengthTextValue extends TextValue {
    
    private $length;
    
    
    public function __construct( $length,
            $value = null, $id = null, $emptyIsValid = false ) {
        
        // Note we need to set the length before calling the parent constructor
        // as it calls validate that depends on a correct length.
        $this->length = $length;
        parent::__construct($value, $id, $emptyIsValid);
    }
    
    protected function validate( $value ) {
        
        $parent = parent::validate($value);
        if(!$parent) {
            return $parent;
        }
        
        $count = strlen($value);
        return $count <= $this->length;
    }
}