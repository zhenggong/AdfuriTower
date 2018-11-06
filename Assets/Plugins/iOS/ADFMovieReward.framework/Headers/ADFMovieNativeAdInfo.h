//
//  ADFMovieNativeAdInfo.h
//  ADFMovieReword
//
//  Created by Toru Furuya on 2017/02/21.
//  (c) 2017 ADFULLY Inc.
//

#import <Foundation/Foundation.h>
#import "ADFMediaView.h"

typedef NS_ENUM(NSInteger, ADFMovieNativeAdType) {
    ADFMovieNativeAdType_Unknown,
    ADFMovieNativeAdType_Movie,
    ADFMovieNativeAdType_Image,
};

/**
 動画ネイティブ広告の情報を格納したオブジェクト
 */
@class ADFmyMovieNativeInterface;
@interface ADFMovieNativeAdInfo : NSObject
@property (nonatomic, weak) ADFmyMovieNativeInterface *adapter;

/**
 動画ネイティブ広告のタイトル
 */
@property (nonatomic, readonly, copy) NSString *title;

/**
 動画ネイティブ広告の説明文
 */
@property (nonatomic, readonly, copy) NSString *desc;

/**
 インプレッションのトラッキング済みかどうか
 */
@property (atomic, readonly) BOOL hasTrackedImpression;

/**
 動画再生のトラッキング済みかどうか
 */
@property (atomic, readonly) BOOL hasTrackedMovieStart;

/**
 動画終了のトラッキング済みかどうか
 */
@property (atomic, readonly) BOOL hasTrackedMovieFinish;

/**
 ネイティブ広告のmediaview
 */
@property (nonatomic) ADFMediaView* mediaView;
 

- (instancetype)initWithVideoUrl:(NSURL *)aVideoUrl
                           title:(NSString *)aTitle
                     description:(NSString *)aDescription;
- (instancetype)init __unavailable;

/**
 インプレッションを計測
 広告（動画・静止画）を表示したら実行してください
 */
- (void)trackImpression;

/**
 動画広告の再生開始を計測
 */
- (void)trackMovieStart;

/**
 動画広告の再生終了を計測
 */
- (void)trackMovieFinish;

/**
 広告のクリックを計測
 ユーザが広告をクリックしたら実行してください
 SafariやAppStoreを起動します
 */
- (void)launchClickTarget;

- (void)setupMediaView:(UIView *)view;
- (void)setupMediaView:(NSURL *)imageUrl movieUrl:(NSURL *)movieUrl;
- (void)playMediaView;

@end
