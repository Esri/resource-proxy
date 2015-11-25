<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'CheckedValue.php';


class SelectionValue extends CheckedValue {
    
    private $validValues;
    
    /*
     * $validValues is an associative array. The array keys specify the values
     * the SelectionValue can specify one of. The values are not interpreted by
     * this class. They can be used e.g. as display labels.
     */
    public function __construct( array $validValues,
            $value = null, $id = null, $emptyIsValid = false ) {
        
        $this->validValues = $validValues;
        parent::__construct( $value, $id, $emptyIsValid );
    }
    
    protected function validate( $value ) {
        return in_array( $value, $this->validValues ); 
    }
    
    public function validValues() {
        return $this->validValues;
    }
    
    // each selection value has its own id
    public function valueId( $val = null ) {
        
        $value = $val ?: $this->get();
        if( in_array( $value, $this->validValues ) ) {
            return $this->id() . '-' . $value;
        }
    }
}
