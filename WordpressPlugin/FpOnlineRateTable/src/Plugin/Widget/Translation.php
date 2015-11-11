<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin\Widget;

/**
 * Description of Translation
 *
 * @author scharfenberg
 */
abstract class Translation {
    
    static public function getTexts() {
        
        $result = [
            'selectCountry' => _x('Select Country', 'Start Calculation Page', 'FpOnlinerateTable'),
            'selectSenderZip' => _x('Sender Zip Code', 'Start Calculation Page', 'FpOnlinerateTable')
        ];
        
        return $result;
    }
}
