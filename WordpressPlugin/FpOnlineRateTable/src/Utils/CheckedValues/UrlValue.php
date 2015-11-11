<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'CheckedValue.php';
require_once dirname(dirname(dirname(__DIR__))) . '/vendor/autoload.php';

use Respect\Validation\Validator;


class UrlValue extends CheckedValue {
    
    protected function validate($value) {
        return Validator::url()->validate($value);
    }
}