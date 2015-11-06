<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

$includePath = dirname(__FILE__);

require_once 'SelectionValue.php';
require_once $includePath . '/Helper/CallableHelper.php';
require_once $includePath . '/Polyfill/array_column.php';    // if we are not using PHP 5.5 or greater we need to include this file

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\CallableHelper;


/**
 * Description of Currencies
 * According to ISO 4217
 *
 * @author scharfenberg
 */
class Currency extends SelectionValue {
    
    private $translationTable;
    
    
    static private $defaultTranslationTable = [
        'CAD' => ['Canadian dollar', 'Canadian dollars'],
        'DKK' => ['Dansk krone', 'Danske kroner'],
        'NOK' => ['Norsk krone', 'Norske kroner'],
        'RUB' => ['российский рубль', 'российский рубль'],
        'SEK' => ['Svensk krona', 'Svensk kronor'],
        'GBP' => ['Englisch Pound', 'Englisch Pounds'],
        'USD' => ['United States Dollar', 'United States Dollars'],
        'EUR' => ['Euro', 'Euros'] ];
    
    static private function currencyTable() {
        
        static $currencies = null;
        
        if( !isset( $currencies ) ) {
            $currencies = [
                [ 'iso' => 'CAD', 'number' => 124 ],
                [ 'iso' => 'DKK', 'number' => 208 ],
                [ 'iso' => 'NOK', 'number' => 578 ],
                [ 'iso' => 'RUB', 'number' => 643 ],
                [ 'iso' => 'SEK', 'number' => 752 ],
                [ 'iso' => 'GBP', 'number' => 826 ],
                [ 'iso' => 'USD', 'number' => 840 ],
                [ 'iso' => 'EUR', 'number' => 978 ] ];
        }
        
        return $currencies;
    }
    
    static public function iso4217_codes() {
        
        static $codes = null;
        
        if( !isset( $codes ) ) {
            $codes = array_column( self::currencyTable(), 'iso' );
        }
        
        return $codes;
    }
    
    public function __construct( $iso4217, $translationTable = null ) {
        
        if( isset( $translationTable ) ) {
            $this->translationTable = $translationTable;
        } else {
            $this->translationTable = self::$defaultTranslationTable;
        }
        
        parent::__construct( self::iso4217_codes(), $iso4217 );
    }
    
    public function getSingularName() {
        return CallableHelper::callOrReturn(
                $this->translationTable[$this->get()][0] );
    }
    
    public function getPluralName() {
        return CallableHelper::callOrReturn(
                $this->translationTable[$this->get()][1] );
    }
}
