<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'SelectionValue.php';


class IntegerSelectionValue extends SelectionValue {
    
    protected function sanitize( $value ) {
        return (int) parent::sanitize( $value );
    }
}
