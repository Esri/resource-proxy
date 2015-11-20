<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

$includePath = dirname( dirname( __FILE__ ) );

require_once dirname(__DIR__) . '/GlobalLogger.php';
require_once 'ICustomCssWrapperConfig.php';
require_once 'Helper.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\ICustomCssWrapperConfig;


/**
 * A class to include Css files into Wordpress plugins
 *
 * @author scharfenberg
 */
class CustomCssWrapper {
   
    const DEFAULT_ID_PREFIX = 'fp-custom-css-wrapper-';
    
    static private $idPrefix = self::DEFAULT_ID_PREFIX;
    
    private $handle;
    private $source;
    private $dependencies;
    
    
    public function handle() {
        return $this->handle;
    }
    
    //
    // Enqueue this css now.
    // This method is used internally by the action handlers of this class but can also
    // be called by external entities if they implement their own action handling.
    //
    public function load() {
        wp_enqueue_style($this->handle());
    }
    
    public function loadOnAction($action = 'wp_enqueue_scripts') {
        add_action($action, [$this, 'load']);
    }
    
    public function register() {
        wp_register_style($this->handle, $this->source, $this->dependencies);
    }
    
    public function register_callback() {
        
        try {
            $this->register();
        } catch ( \Exception $ex ) {
            GlobalLogger::addError( $ex );
        }
    }
    
    public function registerOnAction($action = 'init') {
        add_action($action, [$this, 'register_callback']);
    }
    
    public function registerAndLoad() {
        
        $this->register();
        $this->load();
    }
    
    public function setDependencies(array $deps) {
        $this->dependencies = self::transformWordpressDepencies($deps);
    }
    
    public function __construct($source, array $dependencies = []) {
        
        if(!is_string($source)) {
            throw new \ArgumentException(
                    'CustomCssWrapper source code path need to be of type string');
        }
         
        $this->handle = self::$idPrefix . $source;
        $this->source = $source;
        $this->setDependencies($dependencies);
    }
    
    
    static public function readConfiguration(
            ICustomCssWrapperConfig $config) {
        self::$idPrefix = $config->idPrefix();
    }
    
    private static function transformWordpressDepencies(array $dependencies) {
        
        $wpDeps = array_map(
                function(CustomCssWrapper $item) {
                    return $item->handle(); },
                $dependencies);
        
        return $wpDeps;
    }
}

