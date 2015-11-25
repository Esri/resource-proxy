<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;

require_once 'Country.php';
require_once 'Language.php';
require_once 'CheckedValue.php';


class Culture extends CheckedValue {
    
    protected function validate( $value ) {
        
        $value = str_replace( '-', '_', $value );
        $elements = explode( '_', $value );
        
        if( count( $elements ) !== 2 ) {
            return false;
        }
        
        if( !in_array( $elements[0], Language::iso639_1_codes() ) ) {
            return false;
        }
        
        if( !in_array( $elements[1], Country::iso3166a2_codes() ) ) {
            return false;
        }
        
        return true;
    }
    
    public function getCountry() {
        
        $iso3166a2 = substr( $this->get(), -2 );
        return Country::createFromIso3166a2( $iso3166a2 );
    }
    
    public function getLanguage() {
        
        $iso639_1 = substr( $this->get(), 0, 2 );
        return Language::createFromIso639_1( $iso639_1 );
    }
    
    public function getUsingSeparator( $sep ) {
        
        $result = str_replace( '_', $sep, $this->get() );
        return $result;
    }
    
    static public function createFromCountryAndLanguage(
            Country $country, Language $language ) {
        
        $culture = $language->getIso639_1() . '_' . $country->getIso3166a2();
        return new self( $culture );
    }
    
    static public function createFromString( $culture ) {
        return new self( $culture );
    }
}