<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;


class WordpressException extends \RuntimeException {
    public function __construct($error, $previous = null) {
        
        if($error instanceof \WP_Error) {
            $msg = $error->get_error_message();
        } else {
            $msg = $error;
        }
        
        parent::__construct($msg, 0, $previous);
    }
}


/**
 * Description of Helper
 *
 * @author scharfenberg
 */
abstract class Helper {

    static public function ifWPErrorThrow($thing) {
        if(is_wp_error($thing)) {
            throw new WordpressException($thing);
        }
    }
    
    static public function ifFalseThrow($thing, $function) {
        if(false === $thing) {
            throw new WordpressException(
                    "Wordpress function '{$function}' returned without success");
        }
    }
}
