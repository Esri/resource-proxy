<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

class TokenIterator extends \Iterator {
    
    public function current() {
        
    }
    
    public function key() {
        
    }
    
    public function next() {
        
    }
    
    public function rewind() {
        
    }
    
    public function valid() {
        
    }
}


$directories = [
    dirname(__DIR__) . '/vendor'
];

foreach($directories as $directory) {
    
    $directoryIterator = new RecursiveDirectoryIterator($directory);
    $objects = new RecursiveIteratorIterator($directoryIterator);
    foreach($objects as $object) {
        if(('file' === $object->getType())
                && ('php' === $object->getExtension())) {
            $sourceCode = file_get_contents($object->getRealPath());
            $tokens = token_get_all($sourceCode);
            foreach($tokens as $token) {
                if(!is_string($token)) {
                    list($id, $text) = $token;
                    if(T_NAMESPACE === $id) {
                        echo "found namespace: {$text}<br/>";
                    }
                }
            }
        }
    }
}