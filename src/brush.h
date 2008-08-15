/* -*- Mode: C++; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * brush.h: Brushes
 *
 * Contact:
 *   Moonlight List (moonlight-list@lists.ximian.com)
 *
 * Copyright 2007 Novell, Inc. (http://www.novell.com)
 *
 * See the LICENSE file included with the distribution for details.
 * 
 */

#ifndef __BRUSH__H__
#define __BRUSH__H__

#include <glib.h>

G_BEGIN_DECLS

#include "enums.h"
#include "collection.h"
#include "uielement.h"

class MediaElement;

enum AlignmentX {
	AlignmentXLeft,
	AlignmentXCenter,
	AlignmentXRight
};

enum AlignmentY {
	AlignmentYTop,
	AlignmentYCenter,
	AlignmentYBottom
};

enum BrushMappingMode {
	BrushMappingModeAbsolute,
	BrushMappingModeRelativeToBoundingBox
};

enum ColorInterpolationMode {
	ColorInterpolationModeScRgbLinearInterpolation,
	ColorInterpolationModeSRgbLinearInterpolation
};

enum GradientSpreadMethod {
	GradientSpreadMethodPad,
	GradientSpreadMethodReflect,
	GradientSpreadMethodRepeat
};


/* @Namespace=System.Windows.Media */
class Brush : public DependencyObject {
 protected:
	virtual ~Brush () {}

 public:
	/* @PropertyType=double,DefaultValue=1.0 */
	static DependencyProperty *OpacityProperty;
	/* @PropertyType=Transform */
	static DependencyProperty *RelativeTransformProperty;
	/* @PropertyType=Transform */
	static DependencyProperty *TransformProperty;
	
	// internal property - generic brush property change
	// used only for notifying attachees
	/* @PropertyType=bool,Access=Internal */
	static DependencyProperty *ChangedProperty;
	
	/* @GenerateCBinding,GeneratePInvoke,ManagedAccess=Protected */
	Brush () { }
	
	virtual Type::Kind GetObjectType () { return Type::BRUSH; };
	
	virtual void SetupBrush (cairo_t *cr, UIElement *uielement);
	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);

	virtual void OnSubPropertyChanged (DependencyProperty *prop, DependencyObject *obj, PropertyChangedEventArgs *subobj_args);

	// returns true if OpacityProperty == 1.0.
	// subclasses override this to deal with their local coloring
	virtual bool IsOpaque ();
	
	//
	// Property Accessors
	//
	void SetOpacity (double opacity);
	double GetOpacity ();
	
	void SetRelativeTransform (Transform *transform);
	Transform *GetRelativeTransform ();
	
	void SetTransform (Transform *transform);
	Transform *GetTransform ();
};


/* @Namespace=System.Windows.Media */
class SolidColorBrush : public Brush {
 protected:
	virtual ~SolidColorBrush () {}
	
 public:
	/* @PropertyType=Color,DefaultValue=Color (0x00000000) */
	static DependencyProperty *ColorProperty;
	
	/* @GenerateCBinding,GeneratePInvoke */
	SolidColorBrush () { }
	SolidColorBrush (const char *color);
	
	virtual Type::Kind GetObjectType () { return Type::SOLIDCOLORBRUSH; };

	virtual void SetupBrush (cairo_t *cr, UIElement *uielement);
	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);
	
	virtual bool IsOpaque ();
	
	//
	// Property Accessors
	//
	void SetColor (Color *color);
	Color *GetColor ();
};


/* @Namespace=System.Windows.Media */
class GradientStopCollection : public DependencyObjectCollection {
 protected:
	virtual ~GradientStopCollection () {}

 public:
	/* @GenerateCBinding,GeneratePInvoke */
	GradientStopCollection () { }
	
	virtual Type::Kind GetObjectType () { return Type::GRADIENTSTOP_COLLECTION; }
	virtual Type::Kind GetElementType() { return Type::GRADIENTSTOP; }
};


/* @Namespace=System.Windows.Media */
class GradientStop : public DependencyObject {
 protected:
	virtual ~GradientStop () {}
	
 public:
	/* @PropertyType=Color,DefaultValue=Color (0x00000000) */
	static DependencyProperty *ColorProperty;
	/* @PropertyType=double,DefaultValue=0.0 */
	static DependencyProperty *OffsetProperty;
	
	/* @GenerateCBinding,GeneratePInvoke */
	GradientStop () { }
	
	virtual Type::Kind GetObjectType () { return Type::GRADIENTSTOP; }
	
	//
	// Property Accessors
	//
	void SetColor (Color *color);
	Color *GetColor ();
	
	void SetOffset (double offset);
	double GetOffset ();
};


// note: abstract in C#
/* @Namespace=System.Windows.Media */
/* @ContentProperty="GradientStops" */
class GradientBrush : public Brush {
 protected:
	virtual ~GradientBrush () {}

 public:
	/* @PropertyType=ColorInterpolationMode,DefaultValue=ColorInterpolationModeSRgbLinearInterpolation */
	static DependencyProperty *ColorInterpolationModeProperty;
	/* @PropertyType=GradientStopCollection */
	static DependencyProperty *GradientStopsProperty;
	/* @PropertyType=BrushMappingMode,DefaultValue=BrushMappingModeRelativeToBoundingBox */
	static DependencyProperty *MappingModeProperty;
	/* @PropertyType=GradientSpreadMethod,DefaultValue=GradientSpreadMethodPad */
	static DependencyProperty *SpreadMethodProperty;
	
	/* @GenerateCBinding,GeneratePInvoke,ManagedAccess=Protected */
	GradientBrush ();
	
	virtual Type::Kind GetObjectType () { return Type::GRADIENTBRUSH; }
	
	virtual void OnCollectionItemChanged (Collection *col, DependencyObject *obj, PropertyChangedEventArgs *args);
	virtual void OnCollectionChanged (Collection *col, CollectionChangedEventArgs *args);
	virtual void SetupGradient (cairo_pattern_t *pattern, UIElement *uielement, bool single = false);
	
	virtual bool IsOpaque ();
	
	//
	// Property Accessors
	//
	void SetColorInterpolationMode (ColorInterpolationMode mode);
	ColorInterpolationMode GetColorInterpolationMode ();
	
	void SetGradientStops (GradientStopCollection *collection);
	GradientStopCollection *GetGradientStops ();
	
	void SetMappingMode (BrushMappingMode mode);
	BrushMappingMode GetMappingMode ();
	
	void SetSpreadMethod (GradientSpreadMethod method);
	GradientSpreadMethod GetSpreadMethod ();
};


/* @Namespace=System.Windows.Media */
class LinearGradientBrush : public GradientBrush {
 protected:
	virtual ~LinearGradientBrush () {}

 public:
	/* @PropertyType=Point */
	static DependencyProperty *EndPointProperty;
	/* @PropertyType=Point */
	static DependencyProperty *StartPointProperty;

	/* @GenerateCBinding,GeneratePInvoke */
	LinearGradientBrush () { }
	
	virtual Type::Kind GetObjectType () { return Type::LINEARGRADIENTBRUSH; }

	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);
	
	//
	// Property Accessors
	//
	void SetEndPoint (Point *point);
	Point *GetEndPoint ();
	
	void SetStartPoint (Point *point);
	Point *GetStartPoint ();
};


/* @Namespace=System.Windows.Media */
class RadialGradientBrush : public GradientBrush {
 protected:
	virtual ~RadialGradientBrush () {}

 public:
	/* @PropertyType=Point */
	static DependencyProperty *CenterProperty;
	/* @PropertyType=Point */
	static DependencyProperty *GradientOriginProperty;
	/* @PropertyType=double,DefaultValue=0.5 */
	static DependencyProperty *RadiusXProperty;
	/* @PropertyType=double,DefaultValue=0.5 */
	static DependencyProperty *RadiusYProperty;
	
	/* @GenerateCBinding,GeneratePInvoke */
	RadialGradientBrush () { }
	
	virtual Type::Kind GetObjectType () { return Type::RADIALGRADIENTBRUSH; }
	
	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);
	
	//
	// Property Accessors
	//
	void SetCenter (Point *center);
	Point *GetCenter ();
	
	void SetGradientOrigin (Point *origin);
	Point *GetGradientOrigin ();
	
	void SetRadiusX (double radius);
	double GetRadiusX ();
	
	void SetRadiusY (double radius);
	double GetRadiusY ();
};


/* @Namespace=System.Windows.Media */
class TileBrush : public Brush {
 protected:
	virtual ~TileBrush () {}

 public:
	/* @PropertyType=AlignmentX,DefaultValue=AlignmentXCenter */
	static DependencyProperty *AlignmentXProperty;
	/* @PropertyType=AlignmentY,DefaultValue=AlignmentYCenter */
	static DependencyProperty *AlignmentYProperty;
	/* @PropertyType=Stretch,DefaultValue=StretchFill */
	static DependencyProperty *StretchProperty;
	
	/* @GenerateCBinding,GeneratePInvoke,ManagedAccess=Protected */
	TileBrush () { }
	
	virtual Type::Kind GetObjectType () { return Type::TILEBRUSH; }

	//
	// Property Accessors
	//
	void SetAlignmentX (AlignmentX alignment);
	AlignmentX GetAlignmentX ();
	
	void SetAlignmentY (AlignmentY alignment);
	AlignmentY GetAlignmentY ();
	
	void SetStretch (Stretch stretch);
	Stretch GetStretch ();
};


/* @Namespace=System.Windows.Media */
class ImageBrush : public TileBrush {
	static void image_progress_changed (EventObject *sender, EventArgs *calldata, gpointer closure);
	static void image_failed (EventObject *sender, EventArgs *calldata, gpointer closure);
	
	Image *image;
	
 protected:
	virtual ~ImageBrush ();

 public:
	/* @PropertyType=double,DefaultValue=0.0,ManagedAccess=Private */
	static DependencyProperty *DownloadProgressProperty;
 	/* @PropertyType=string,DefaultValue=\"\",ManagedPropertyType=ImageSource */
	static DependencyProperty *ImageSourceProperty;
	
	const static int DownloadProgressChangedEvent;
	const static int ImageFailedEvent;
	
	/* @GenerateCBinding,GeneratePInvoke */
	ImageBrush ();
	
	virtual Type::Kind GetObjectType () { return Type::IMAGEBRUSH; }
	
	void SetSource (Downloader *downloader, const char *PartName);
	virtual void OnPropertyChanged (PropertyChangedEventArgs *args);
	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);
	virtual void SetSurface (Surface *surface);
	
	virtual bool IsOpaque ();
	
	//
	// Property Accessors
	//
	void SetDownloadProgress (double progress);
	double GetDownloadProgress ();
	
	void SetImageSource (const char *source);
	const char *GetImageSource ();
};

cairo_surface_t *image_brush_create_similar     (cairo_t *cr, int width, int height);


/* @Namespace=System.Windows.Media */
class VideoBrush : public TileBrush {
	MediaElement *media;
	
 protected:
	virtual ~VideoBrush ();
	
 public:
	/* @PropertyType=string,DefaultValue=\"\" */
	static DependencyProperty *SourceNameProperty;
	
	/* @GenerateCBinding,GeneratePInvoke */
	VideoBrush ();
	
	virtual Type::Kind GetObjectType () { return Type::VIDEOBRUSH; }
	
	virtual void OnPropertyChanged (PropertyChangedEventArgs *args);
	virtual void OnSubPropertyChanged (DependencyProperty *prop, DependencyObject *obj, PropertyChangedEventArgs *subobj_args);
	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);

	virtual bool IsOpaque ();
	
	//
	// Property Accessors
	//
	void SetSourceName (const char *name);
	const char *GetSourceName ();
};


/* @Namespace=None */
/* @ManagedDependencyProperties=None */
class VisualBrush : public TileBrush {
	cairo_surface_t *surface;

	static void update_brush (EventObject *, EventArgs *, gpointer closure);

 protected:
	virtual ~VisualBrush () {}

 public:
	/* @PropertyType=UIElement */
	static DependencyProperty *VisualProperty;
	
	/* @GenerateCBinding,GeneratePInvoke */
	VisualBrush () { }
	
	virtual Type::Kind GetObjectType () { return Type::VISUALBRUSH; };

	virtual void SetupBrush (cairo_t *cr, UIElement *uielement, double width, double height);
	virtual void OnPropertyChanged (PropertyChangedEventArgs *args);

	virtual bool IsOpaque ();
	
	//
	// Property Accessors
	//
	void SetVisual (UIElement *visual);
	UIElement *GetVisual ();
};

G_END_DECLS

#endif /* __BRUSH_H__ */
