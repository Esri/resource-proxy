<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once dirname(__DIR__) . '/GlobalLogger.php';
require_once 'JavascriptWrapper.php';
require_once 'ICustomJavascriptWrapperConfig.php';
require_once 'Helper.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;
use FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress\ICustomJavascriptWrapperConfig;


/**
 * Description of CutomJavascriptWrapper
 *
 * @author scharfenberg
 */
class CustomJavascriptWrapper extends JavascriptWrapper
{
    const DEFAULT_ID_PREFIX = 'fp_custom_javascript_wrapper-';
    const DEFAULT_LOCALIZATION_OBJECT = 'localization';
    const DEFAULT_META_OBJECT = 'meta';
    
    static private $idPrefix = self::DEFAULT_ID_PREFIX;
    static private $localizationObject = self::DEFAULT_LOCALIZATION_OBJECT;
    static private $metaObject = self::DEFAULT_META_OBJECT;
    
    private $source;
    private $dependencies;
    private $texts;
    private $inFooter;
    private $metaInfo;
    
    
    private function wpRegisterScript() {
        wp_register_script(
                $this->handle(),
                $this->source,
                $this->dependencies,
                null,
                $this->inFooter
        );
    }
    
    private function wpLocalizeScript() {
        
        if(count($this->texts) > 0) {
            $result = wp_localize_script(
                    $this->handle(),
                    self::$localizationObject,
                    $this->texts
            );
            Helper::ifFalseThrow($result, "wp_localize_script");
        }
    }

    
    // 'misuse' wp_localize_script to pass additional meta information
    // to the script
    private function loadMetaInfo() {
        
        if(count($this->metaInfo) > 0) {
            $result = wp_localize_script(
                    $this->handle(),
                    self::$metaObject,
                    $this->metaInfo
            );
            Helper::ifFalseThrow($result, "wp_localize_script");
        }
    }
    
    public function register() {
        
        $this->wpRegisterScript();
        $this->wpLocalizeScript();
        $this->loadMetaInfo();
    }
    
    public function registerAndLoad() {
        
        $this->register();
        $this->load();
    }
    
    public function registerOnAction($action = 'init', $inFooter = false) {
        
        $this->inFooter = $inFooter;
        add_action($action, [$this, 'register_callback'] );
    }
    
    public function register_callback() {
        
        try {
            $this->register($this->inFooter);
        } catch (\Exception $ex) {
            GlobalLogger::addError($ex);
        }
    }
    
    public function __construct(
            $source,
            array $dependencies = [],
            array $texts = [],
            array $metaInfo = [],
            $inFooter = false ) {
        
        if(!is_string($source)) {
            throw new ArgumentException(
                    'CustomJavascript source code path need to be of type string');
        }
        
        parent::__construct( self::$idPrefix . $source );
                
        $this->source = $source;
        $this->setDependencies($dependencies);
        $this->setLocalization($texts);
        $this->setMetaInfo($metaInfo);
        $this->setInFooter($inFooter);
    }
    
    public function setDependencies(array $deps) {
        $this->dependencies = self::transformWordpressDepencies($deps);
    }
    
    public function setLocalization(array $texts) {
        $this->texts = $texts;
    }
    
    public function setMetaInfo(array $metaInfo) {
        $this->metaInfo = $metaInfo;
    }
    
    public function setInFooter($flag) {
        $this->inFooter = $flag;
    }
    
    
    static public function readConfiguration(
            ICustomJavascriptWrapperConfig $config) {
        
        self::$idPrefix = $config->idPrefix();
        self::$localizationObject = $config->localizationObject();
        self::$metaObject = $config->metaObject();
    }
}
