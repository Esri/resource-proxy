<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

$includePath = dirname(__DIR__);

require_once 'Currency.php';
require_once 'SelectionValue.php';
require_once $includePath . '/Helper/CallableHelper.php';
require_once $includePath . '/Polyfill/array_column.php';    // if we are not using PHP 5.5 or greater we need to include this file

use FP\Web\Portal\FpOnlineRateTable\src\Utils\Helper\CallableHelper;


/**
 * Description of Countries
 * According to ISO 3166
 *
 * @author scharfenberg
 */
class Country extends SelectionValue {
    
    private $translationTable;
    
    
    static private $defaultTranslationTable = [
        'AUT' => 'Östereich',
        'BEL' => 'Belgique',
        'CAN' => 'Canada',
        'DNK' => 'Danmark',
        'FRA' => 'France',
        'DEU' => 'Deutschland',
        'ITA' => 'Italia',
        'NLD' => 'Nederland',
        'NOR' => 'Norge',
        'RUS' => 'Россия',
        'SWE' => 'Sverige',
        'GBR' => 'United Kingdom',
        'USA' => 'United States of America' ];
    
    static private function countryTable() {
        
        static $countries = null;
        
        if(!isset($countries)) {
            $countries = [
                [ 'a2' => 'AT', 'a3' => 'AUT', 'number' => 040, 'iso4217' => 'EUR' ],
                [ 'a2' => 'BE', 'a3' => 'BEL', 'number' => 056, 'iso4217' => 'EUR' ],
                [ 'a2' => 'CA', 'a3' => 'CAN', 'number' => 124, 'iso4217' => 'CAD' ],
                [ 'a2' => 'DK', 'a3' => 'DNK', 'number' => 208, 'iso4217' => 'DKK' ],
                [ 'a2' => 'FR', 'a3' => 'FRA', 'number' => 250, 'iso4217' => 'EUR' ],
                [ 'a2' => 'DE', 'a3' => 'DEU', 'number' => 276, 'iso4217' => 'EUR' ],
                [ 'a2' => 'IT', 'a3' => 'ITA', 'number' => 380, 'iso4217' => 'EUR' ],
                [ 'a2' => 'NL', 'a3' => 'NLD', 'number' => 528, 'iso4217' => 'EUR' ],
                [ 'a2' => 'NO', 'a3' => 'NOR', 'number' => 578, 'iso4217' => 'NOK' ],
                [ 'a2' => 'RU', 'a3' => 'RUS', 'number' => 643, 'iso4217' => 'RUB' ],
                [ 'a2' => 'SE', 'a3' => 'SWE', 'number' => 752, 'iso4217' => 'SEK' ],
                [ 'a2' => 'GB', 'a3' => 'GBR', 'number' => 826, 'iso4217' => 'GBP' ],
                [ 'a2' => 'US', 'a3' => 'USA', 'number' => 840, 'iso4217' => 'USD' ] ];
        }
    
        return $countries;
    }
       
    static public function iso3166a3_codes() {
        
        static $codes = null;
        
        if(!isset($codes)) {
            $codes = array_column(self::countryTable(), 'a3');
        }
        
        return $codes;
    }
    
    static public function iso3166a2_codes() {
        
        static $codes = null;
        
        if(!isset($codes)) {
            $codes = array_column(self::countryTable(), 'a2');
        }
        
        return $codes;
    }
    
    static public function iso3166a3_to_iso3166a2_mapping() {
        
        static $mapping = null;
        
        if(!isset($mapping)) {
            $mapping = array_column(self::countryTable(), 'a2', 'a3');
        }
        
        return $mapping;
    }
    
    static public function iso3166a3_to_iso3166Number_mapping() {
        
        static $mapping = null;
        
        if(!isset($mapping)) {
            $mapping = array_column(self::countryTable(), 'number', 'a3');
        }
        
        return $mapping;
    }
    
    static public function iso3166a2_to_iso3166a3_mapping() {
        
        static $mapping = null;
        
        if(!isset($mapping)) {
            $mapping = array_column(self::countryTable(), 'a3', 'a2');
        }
        
        return $mapping;
    }
    
    static public function iso3166a3_to_iso4217_mapping() {
        
        static $mapping = null;
        
        if(!isset($mapping)) {
            $mapping = array_column(self::countryTable(), 'iso4217', 'a3');
        }
        
        return $mapping;
    }
    
    static public function iso3166a2_from_iso3166a3($a3) {
        return self::iso3166a3_to_iso3166a2_mapping()[$a3];        
    }
    
    static public function iso3166a3_from_iso3166a2($a2) {
        return self::iso3166a2_to_iso3166a3_mapping()[$a2];        
    }
    
    static public function iso3166Number_from_iso3166a3($a3) {
        return self::iso3166a3_to_iso3166Number_mapping()[$a3];
    }
    
    static public function iso4217_from_iso3166a3($a3) {
        return self::iso3166a3_to_iso4217_mapping()[$a3];        
    }
    
        
    public function __construct($a3, $translationTable = null) {
        
        if(isset($translationTable)) {
            $this->translationTable = $translationTable;
        } else {
            $this->translationTable = self::$defaultTranslationTable;
        }
        
        parent::__construct(self::iso3166a3_codes(), $a3);
    }
    
    public function getCurrency() {
        return new Currency( self::iso4217_from_iso3166a3($this->get()));
    }
    
    public function getIso3166a2() {
        return self::iso3166a2_from_iso3166a3($this->get());
    }
    
    public function getIso3166Number() {
        return self::iso3166Number_from_iso3166a3($this->get());
    }
    
    public function getIso3166a3() {
        return $this->get();
    }
    
    public function getName() {
        return CallableHelper::callOrReturn(
                $this->translationTable[$this->get()]);
    }
    
    static public function createFromIso3166a2($a2) {
        return new self(self::iso3166a3_from_iso3166a2($a2));
    }
    
    static public function createFromIso3166a3($a3) {
        return new self($a3);
    }
}
