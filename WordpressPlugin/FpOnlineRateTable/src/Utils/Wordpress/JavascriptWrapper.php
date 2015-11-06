<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once 'Helper.php';
require_once dirname(__DIR__) . '/GeneralExceptions.php';

use FP\Web\Portal\FpOnlineRateTable\Utils\ArgumentException;


/**
 * A class to include Javascript files into Wordpress plugins
 *
 * @author scharfenberg
 */
class JavascriptWrapper {
    
    private $handle;
    
    
    public function handle() {
        return $this->handle;
    }
    
    //
    // Enqueue this script now.
    // This method is used internally by the acrion handlers of this class but can also
    // be called by external entities if they implement their own action handling.
    //
    public function load() {
        wp_enqueue_script($this->handle());
    }
    
    public function loadOnAction($action = 'wp_enqueue_scripts') {
        add_action($action, [$this, 'load'] );
    }
    
    public function __construct( $handle )
    {   
        if( is_string( $handle ) ) {
            $this->handle = $handle;
        } else {
            throw new ArgumentException('Javascript handle must be a string');
        }
    }
    
    
    protected static function transformWordpressDepencies(array $dependencies) {
        
        $wpDeps = array_map(
                function(JavascriptWrapper $item) {
                    return $item->handle(); },
                $dependencies);
        
        return $wpDeps;
    }
}

