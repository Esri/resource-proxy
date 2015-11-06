<?php

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

namespace FP\Web\Portal\FpOnlineRateTable\src\Utils\Wordpress;

require_once 'Helper.php';
require_once dirname(__DIR__) . '/GlobalLogger.php';

use FP\Web\Portal\FpOnlineRateTable\src\Utils\GlobalLogger;


/**
 * Description of LocalizationProvider
 *
 * @author scharfenberg
 */
class LocalizationTextDomain {
    
    private $languageDir;
    private $translationDomain;
    
    
    public function load() {
        
        $result = load_plugin_textdomain(
            $this->translationDomain,
            false,
            $this->languageDir
        );
        if(false === $result) {
            GlobalLogger::addWarning(
                    "Localization file could not be loaded",
                    [   'textDomain' => $this->translationDomain,
                        'languageDir' => $this->languageDir]);
        }
    }
    
    public function load_callback() {
        
        try {
            $this->load();
        } catch (Exception $ex) {
            GlobalLogger::addError($ex);
        }
    }
        
    public function loadOnAction($action = 'init') {
        add_action($action, [$this, 'load_callback']);
    }
    
    // Note $languageDir has to be specified as a relative path with
    // respect to the Wordpress plugin directory (e.g. 'TDCInfo/Languages')
    public function __construct($languageDir, $domain) {
    
        $this->languageDir = $languageDir;
        
        // Note: Wordpress looks for translation files in the language
        // directory that look like "<translationDomain>-de_DE.mo".
        $this->translationDomain = $domain; 
    }
}
