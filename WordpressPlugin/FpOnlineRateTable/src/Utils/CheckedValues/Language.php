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
 * Description of Languages
 *
 * @author scharfenberg
 */
class Language extends SelectionValue {
    
    private $translationTable;
    
    
    static private $defaultTranslationTable = [
        'dan' => 'Dansk',
        'deu' => 'Deutsch',
        'eng' => 'English',
        'fra' => 'Français',
        'ita' => 'Italiano',
        'nld' => 'Nederlands',
        'nor' => 'Norsk',
        'rus' => 'Русский язык',
        'swe' => 'Svenska' ];
    
    static private function languageTable() {
        
        static $languages = null;
        
        if( !isset( $languages ) ) {
            $languages = [
                [ 'iso639-1' => 'da', 'iso639-2' => 'dan', 'language' => 'Dansk' ],
                [ 'iso639-1' => 'de', 'iso639-2' => 'deu', 'language' => 'Deutsch' ],
                [ 'iso639-1' => 'en', 'iso639-2' => 'eng', 'language' => 'English' ],
                [ 'iso639-1' => 'fr', 'iso639-2' => 'fra', 'language' => 'Français' ],
                [ 'iso639-1' => 'it', 'iso639-2' => 'ita', 'language' => 'Italiano' ],
                [ 'iso639-1' => 'nl', 'iso639-2' => 'nld', 'language' => 'Nederlands' ],
                [ 'iso639-1' => 'no', 'iso639-2' => 'nor', 'language' => 'Norsk' ],
                [ 'iso639-1' => 'ru', 'iso639-2' => 'rus', 'language' => 'Русский язык' ],
                [ 'iso639-1' => 'sv', 'iso639-2' => 'swe', 'language' => 'Svenska' ] ];
        }
        
        return $languages;
    }
    
    static public function iso639_1_codes() {
        
        static $codes = null;
        
        if( !isset( $codes ) ) {
            $codes = array_column( self::languageTable(), 'iso639-1' );
        }
        
        return $codes;
    }
    
    static public function iso639_2_codes() {
        
        static $codes = null;
        
        if( !isset( $codes ) ) {
            $codes = array_column( self::languageTable(), 'iso639-2' );
        }
        
        return $codes;
    }
    
    static public function iso639_2_to_iso639_1_mapping() {
        
        static $mapping = null;
        
        if( !isset( $mapping ) ) {
            $mapping = array_column(
                    self::languageTable(), 'iso639-1', 'iso639-2' );
        }
        
        return $mapping;
    }
    
    static public function iso639_1_to_iso639_2_mapping() {
        
        static $mapping = null;
        
        if( !isset( $mapping ) ) {
            $mapping = array_column(
                    self::languageTable(), 'iso639-2', 'iso639-1' );
        }
        
        return $mapping;
    }
    
    static public function iso639_1_from_iso639_2( $iso ) {
        return self::iso639_2_to_iso639_1_mapping()[$iso];
    }
    
    static public function iso639_2_from_iso639_1( $iso ) {
        return self::iso639_1_to_iso639_2_mapping()[$iso];
    }
    
    public function __construct( $iso639_2, $translationTable = null ) {
        
        if( isset( $translationTable ) ) {
            $this->translationTable = $translationTable;
        } else {
            $this->translationTable = self::$defaultTranslationTable;
        }
        
        parent::__construct( self::iso639_2_codes(), $iso639_2 );
    }
    
    public function getIso639_1() {
        return self::iso639_1_from_iso639_2( $this->get() );
    }
    
    public function getIso639_2() {
        return $this->get();
    }
    
    public function getName() {
        return CallableHelper::callOrReturn(
                $this->translationTable[$this->get()] );
    }
    
    static public function createFromIso639_1( $iso639_1 ) {
        return new self( self::iso639_2_from_iso639_1( $iso639_1 ) );
    }
    
    static public function createFromIso639_2( $iso639_2 ) {
        return new self( $iso639_2 );
    }
}
