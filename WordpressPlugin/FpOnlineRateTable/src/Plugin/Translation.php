<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin;

/**
 * Description of Translation
 *
 * @author scharfenberg
 */
abstract class Translation {
    
    static private function getTexts() {
        
        $result = [
            'selectCountry' => _x('SelectCountry', 'Start Calculation Page', 'FpOnlinerateTable')
        ];
    }
}
