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
            'selectSenderZip' => _x('Sender Zip Code', 'Start Calculation Page: Zip input field label', 'FpOnlinerateTable'),
            'startCalculation' => _x('Start Product Calculation', 'Start Calculation Page: Start calculation button label', 'FpOnlinerateTable'),
            'invalidZipCode' => _x('Please enter a valid Zip code', 'Start Calculation Page error text: invalid zip code entered', 'FpOnlinerateTable'),
            'postage' => _x('Postage:', 'During Calculation Page: postage value indicator label', 'FpOnlinerateTable'),
            'weight' => _x('Weight:', 'During Calculation Page: weight indicator label', 'FpOnlinerateTable'),
            'changeWeight' => _x('Change', 'During Calculation Page: Change weight button label', 'FpOnlinerateTable'),
            'ok' => _x('Ok', 'During Calculation Page: Ok Button lable', 'FpOnlinerateTable'),
            'kg' => _x('kg', 'During Calculation Page: weight value kilogram unit', 'FpOnlinerateTable'),
            'g' => _x('g', 'During Calculation Page: weight value gram unit', 'FpOnlinerateTable'),
            'lb' => _x('lb', 'During Calculation Page: weight value pound unit', 'FpOnlinerateTable'),
            'oz' => _x('oz', 'During Calculation Page: weight value ounce unit', 'FpOnlinerateTable'),
            'notComplete' => _x('not complete', 'During Calculation Page: calculation incomplete indicator', 'FpOnlinerateTable'),
            'stepBack' => _x('Step Back', 'During Calculation Page: button label', 'FpOnlinerateTable'),
            'restart' => _x('Restart', 'During Calculation Page: button label', 'FpOnlinerateTable'),
            'finish' => _x('Finish', 'During Calculation Page: button label', 'FpOnlinerateTable'),
            'history' => _x('History', 'During Calculation Page: history list title', 'FpOnlinerateTable'),
            'errorRestartCalculation' => _x('Restart Product Calculation', 'Calculation Error Page: restart button label', 'FpOnlinerateTable'),
            'complete' => _x('Product Calculation complete', 'Finish page: calculation complete text', 'FpOnlinerateTable')
        ];
        
        return $result;
    }
}
