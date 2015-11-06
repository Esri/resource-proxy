<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\CheckedValue;


abstract class CheckedValue {
    
    private $value = null;      // the value
    private $isValid = false;   // flag if the value is valid
    private $emptyIsValid;      // flag if an empty value is considered as valid
    private $id;                // an id that is used when accessing the value in html fields or in post requests
    
    
    public function __construct(
            $value = null, $id = null, $emptyIsValid = false ) {
        
        $this->emptyIsValid = $emptyIsValid;
        $this->id = $id;
        $this->set( $value );
    }
    
    abstract protected function validate( $value );
    protected function sanitize( $value ) {
        return $value;
    }
    
    public function set( $value ) {
        
        $this->value = $this->sanitize( $value );
        
        if( empty( $value ) && $this->emptyIsValid ) {
            $this->isValid = true;
        } else {
            $this->isValid = $this->validate( $this->value );
        }
    }
    
    public function get() {
        return $this->value;
    }
    
    public function getIfValid() {
        
        if( $this->isValid() ) {
            return $this->get();
        } else {
            return null;
        }
    }
    
    public function getAsString() {
        return (string) $this->get();
    }
    
    public function isValid() {
        return $this->isValid;
    }
    
    public function id() {
        return $this->id;
    }
    
    public function fromArray( array $array ) {
        
        if( isset( $this->id ) && array_key_exists( $this->id, $array ) ) {
            $this->set( $array[$this->id] );
        }
    }
    
    public function fromPost() {
        $this->fromArray( $_POST );
    }
    
    public function fromGet() {
        $this->fromArray( $_GET );
    }
    
    public function fromCookie() {
        $this->fromArray( $_COOKIE );
    }
}