<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Plugin;

// Exit if accessed directly
if (!defined('ABSPATH')) {
    exit;
}

require_once dirname(__DIR__) . '/Utils/GlobalLogger.php';
//require_once dirname(__DIR__) . '/Utils/WordPress/JavascriptWrapper.php';
//require_once dirname(__DIR__) . '/Utils/WordPress/CustomJavascriptWrapper.php';
require_once dirname(__DIR__) . '/Utils/Wordpress/LocalizationTextDomain.php';
require_once 'IWidgetConfig.php';
require_once 'WidgetSettings.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;
//use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\JavascriptWrapper;
//use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\CustomJavascriptWrapper;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\LocalizationTextDomain;


class Widget extends \WP_Widget {
    
    static private $config;
    static private $textDomain;
    
    //private $js;
    
    
    public function __construct() {
        
        $widget_ops = [
            'classname' => self::$config->htmlClass(),
            'description' => _x(
                    'Use this plugin to calculate postage prices',
                    'Widget Description',
                    'FpOnlinerateTable') ];

        parent::__construct(
                self::$config->id(),
                _x('FP Online Rate Table', 'Widget Title', 'FpOnlinerateTable'),
                $widget_ops);
        
        //$this->registerAssets();
    }
    
    /*private function registerAssets() {
        
        $assetsUrl = plugin_dir_url(dirname(__DIR__)) . 'assets/js/';

        $buildInScripts = array();
        $buildInScripts[] = new JavascriptWrapper('jquery');
        
        $customScripts = array();
        $customScripts[] = new CustomJavascriptWrapper($assetsUrl . 'test.js');
        foreach($customScripts as $script) {
            $script->register();
        }
        
        $dependencies = array_merge($buildInScripts, $customScripts);
        
        $dependencies = $buildInScripts;
        $this->js = new CustomJavascriptWrapper(
                $assetsUrl . 'test.js', $dependencies );
    }*/
    
    public function widget($args, $instance) {
        
        try {
            self::$textDomain->load();
            
            // will be used in template
            $widgetSettings = new WidgetSettings($this, $instance);
            include(__DIR__ . '/templates/widget.php');
            
            $this->loadRequireJs();
        } catch(\Exception $ex) {
            GlobalLogger::addError($ex);
        }
    }
    
    public function form($instance) {
        
        // will be used in template
        $widgetSettings = new WidgetSettings($this, $instance);
        include(__DIR__ . '/templates/admin.php');
    }
    
    public function update($new_instance, $old_instance) {
        
        $widgetSettings = new WidgetSettings($this, $old_instance);
        $widgetSettings->mergeArray($new_instance);
        
        return $widgetSettings->toArray();
    }
    

    public static function registerWidget_callback() {
        register_widget(get_class());
    }
    
    public static function shortcode_callback() {
        
        try {        
            // start output buffering
            ob_start();
            
            // render this widget (all output is redirected into the buffer)
            the_widget(get_class());
            
            // extract the buffer content
            $result = ob_get_clean();
            
        } catch(\Exception $ex) {
            GlobalLogger::addError($ex);
        }
        
        return $result;
    }
    
    public static function registerOnAction(IWidgetConfig $config,
            LocalizationTextDomain $textDomain, $action = 'widgets_init') {
        
        self::$config = $config;
        self::$textDomain = $textDomain;
        
        add_action($action, get_class() . '::registerWidget_callback' );
        add_shortcode(self::$config->shortcode(),
                get_class() . '::shortcode_callback' );
    }
    
    public static function config() {
        return self::$config;
    }
    
    
    private function loadRequireJs() {
        
        // load RequireJS within the footer tag of the page. This is done
        // because currently also the scripts managed by RequireJS depend
        // on legacy scripts loaded by standard Wordpress mechanisms inside the
        // footer.
        // RequireJS is the new way of dealing with Javascript dependencies so
        // we do not need CustomJavascriptWrapper for scripts managed through
        // it.
        $jsPath = plugins_url('app/js', dirname(__DIR__));
        $requireJsPath = plugins_url('bower_components/requirejs', dirname(__DIR__));
        add_action( 'wp_footer', function() use ($jsPath, $requireJsPath) {
            echo "<script data-main='{$jsPath}/main' src='{$requireJsPath}/require.js'></script>";
        }, 99);
    }
}